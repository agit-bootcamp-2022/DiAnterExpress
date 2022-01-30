using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DiAnterExpress.Data;
using DiAnterExpress.Dtos;
using DiAnterExpress.Externals;
using DiAnterExpress.Models;
using DiAnterExpress.SyncDataServices.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using NetTopologySuite.Geometries;

namespace DiAnterExpress.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentType _shipmentType;
        private readonly IShipment _shipment;
        private readonly IMapper _mapper;
        private readonly IShipmentInternalDataClient _http;
        private readonly ITransactionInternal _transactionInternal;
        private readonly IBranch _branch;
        private readonly ITokopodiaDataClient _tokopodia;

        public ShipmentController(IShipmentType shipmentType,
            IShipment shipment, IMapper mapper, IShipmentInternalDataClient http,
            ITransactionInternal transactionInternal, IBranch branch, ITokopodiaDataClient tokopodia)
        {
            _shipmentType = shipmentType;
            _shipment = shipment;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _http = http;
            _transactionInternal = transactionInternal;
            _branch = branch;
            _tokopodia = tokopodia;
        }

        [AllowAnonymous]
        [HttpPost("fee")]
        public async Task<ActionResult<ReturnSuccessDto<ShipmentFeeOutput>>> GetShipmentFee(ShipmentFeeInput input)
        {
            try
            {
                var shipmentType = await _shipmentType.GetById(input.ShipmentTypeId);
                if (shipmentType != null)
                {
                    var fee = await _shipment.GetShipmentFee(input, shipmentType.CostPerKm, shipmentType.CostPerKg);
                    return Ok(
                        new ReturnSuccessDto<ShipmentFeeOutput>
                        {
                            data = new ShipmentFeeOutput
                            {
                                Fee = fee
                            }
                        }
                    );
                }
                else
                {
                    return NotFound(
                        new ReturnErrorDto
                        {
                            message = "Shipment Type not found"
                        }
                    );
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(
                    new ReturnErrorDto
                    {
                        message = ex.Message
                    }
                );
            }
        }

        [AllowAnonymous]
        [HttpPost("fee/all")]
        public async Task<ActionResult<ReturnSuccessDto<IEnumerable<DtoShipmentFeeAllType>>>> GetShipmentFeeAllType(ShipmentFeeAllInput input)
        {
            try
            {
                var newInput = _mapper.Map<ShipmentFeeInput>(input);
                var shipmentTypes = await _shipmentType.GetAll();
                var response = new List<DtoShipmentFeeAllType>();
                if (shipmentTypes != null)
                {
                    foreach (var shipmentType in shipmentTypes)
                    {
                        var fee = await _shipment.GetShipmentFee(newInput, shipmentType.CostPerKm, shipmentType.CostPerKg);
                        response.Add(new DtoShipmentFeeAllType
                        {
                            Id = shipmentType.Id,
                            Name = shipmentType.Name,
                            TotalFee = fee
                        });
                    }
                }
                return Ok(
                    new ReturnSuccessDto<IEnumerable<DtoShipmentFeeAllType>>
                    {
                        data = response
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(
                    new ReturnErrorDto
                    {
                        message = ex.Message
                    }
                );
            }
        }

        [Authorize(Roles = "Admin, Branch")]
        [HttpPost("internal")]
        public async Task<ActionResult<ReturnSuccessDto<ShipmentInternalOutput>>> CreateShipmentInternal(ShipmentInternalInput input)
        {
            try
            {
                var shipmentType = await _shipmentType.GetById(input.ShipmentTypeId);
                if (shipmentType != null)
                {
                    var mapCost = new ShipmentFeeInput
                    {
                        SenderLat = input.SenderLocation.Latitude,
                        SenderLong = input.SenderLocation.Longitude,
                        ReceiverLat = input.ReceiverLocation.Latitude,
                        ReceiverLong = input.ReceiverLocation.Longitude,
                        Weight = input.TotalWeight,
                        ShipmentTypeId = input.ShipmentTypeId
                    };
                    var fee = await _shipment.GetShipmentFee(mapCost, shipmentType.CostPerKm, shipmentType.CostPerKg);

                    var loginUser = new LoginUserInput
                    {
                        Username = input.UserCredential.UangTransUsername,
                        Password = input.UserCredential.UangTransPassword
                    };
                    var token = await _http.LoginUser(loginUser);
                    var customerId = await _http.GetProfile(token.Token);

                    var inputHttp = new TransferBalanceDto
                    {
                        CustomerDebitId = customerId.Id,
                        Amount = fee,
                        CustomerCreditId = 5 //5 is AnterAja Id in UangTrans Service
                    };

                    var httpRequest = await _http.CreateShipmentInternal(inputHttp, token.Token);
                    if (httpRequest.Succeed == true)
                    {
                        var transactionInternal = new TransactionInternal
                        {
                            Product = input.Product,
                            PaymentId = httpRequest.ReceiverWalletMutationId
                        };
                        var transactionId = await _transactionInternal.Insert(transactionInternal);

                        var shipment = new Shipment
                        {
                            SenderName = input.SenderName,
                            SenderContact = input.SenderContact,
                            SenderAddress = new Point(input.SenderLocation.Longitude, input.SenderLocation.Latitude) { SRID = 4326 },
                            ReceiverName = input.ReceiverName,
                            ReceiverContact = input.ReceiverContact,
                            ReceiverAddress = new Point(input.ReceiverLocation.Longitude, input.ReceiverLocation.Latitude) { SRID = 4326 },
                            TotalWeight = input.TotalWeight,
                            Cost = fee,
                            Status = Status.OrderReceived,
                            TransactionType = TransactionType.Internal,
                            TransactionId = transactionId.Id,
                            TransactionToken = token.Token,
                            ShipmentTypeId = input.ShipmentTypeId,
                            BranchSrcId = (await _branch.GetNearestByLocation(input.SenderLocation)).Id,
                            BranchDstId = (await _branch.GetNearestByLocation(input.ReceiverLocation)).Id,
                        };

                        var shipmentResult = await _shipment.Insert(shipment);

                        return Ok(
                            new ReturnSuccessDto<ShipmentInternalOutput>
                            {
                                data = new ShipmentInternalOutput
                                {
                                    ShipmentId = shipmentResult.Id,
                                    StatusOrder = shipment.Status.ToString()
                                }
                            }
                        );
                    }
                    else
                    {
                        return BadRequest(
                            new ReturnErrorDto
                            {
                                message = ""
                            }
                        );
                    }
                }
                else
                {
                    return NotFound(
                        new ReturnErrorDto
                        {

                        }
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(
                    new ReturnErrorDto
                    {
                        message = ex.Message
                    }
                );
            }
        }

        [HttpGet]
        public async Task<ActionResult<ReturnSuccessDto<IEnumerable<DtoShipmentOutput>>>> GetAllShipment()
        {
            var result = await _shipment.GetAll();
            var dtos = _mapper.Map<IEnumerable<DtoShipmentOutput>>(result);
            return Ok(
                new ReturnSuccessDto<IEnumerable<DtoShipmentOutput>>
                {
                    data = dtos
                }
            );
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReturnSuccessDto<DtoShipmentOutput>>> GetShipmentById(int id)
        {
            var result = await _shipment.GetById(id);
            if (result == null)
                return NotFound();

            return Ok(
                new ReturnSuccessDto<DtoShipmentOutput>
                {
                    data = _mapper.Map<DtoShipmentOutput>(result)
                }
            );
        }

        [AllowAnonymous]
        [HttpGet("{id}/Status")]
        public async Task<ActionResult<ReturnSuccessDto<DtoStatus>>> GetShipmentStatus(int id)
        {
            var result = await _shipment.GetById(id);
            if (result == null)
                return NotFound();

            return Ok(
                new ReturnSuccessDto<DtoStatus>
                {
                    data = _mapper.Map<DtoStatus>(result)
                }
            );
        }

        [Authorize(Roles = "Admin, Tokopodia")]
        [HttpPost("tokopodia")]
        public async Task<ActionResult<ReturnSuccessDto<DtoShipmentCreateReturn>>> CreateShipmentTokpod([FromBody] DtoShipmentCreateTokopodia input)
        {
            try
            {
                var shipmentType = await _shipmentType.GetById(input.shipmentTypeId);

                var totalFee = await _shipment.GetShipmentFee(
                    new ShipmentFeeInput
                    {
                        SenderLat = input.senderLat,
                        SenderLong = input.senderLong,
                        ReceiverLat = input.receiverLat,
                        ReceiverLong = input.receiverLong
                    },
                    shipmentType.CostPerKm, shipmentType.CostPerKg
                );

                var shipmentObj = new Shipment
                {
                    TransactionId = input.transactionId,
                    TransactionType = TransactionType.Tokopodia,

                    SenderName = input.senderName,
                    SenderContact = input.senderContact,
                    SenderAddress = new Point(input.senderLong, input.senderLat) { SRID = 4326 },

                    ReceiverName = input.receiverName,
                    ReceiverContact = input.receiverContact,
                    ReceiverAddress = new Point(input.receiverLong, input.receiverLat) { SRID = 4326 },

                    TotalWeight = input.totalWeight,
                    Cost = totalFee,
                    Status = Status.OrderReceived,
                    ShipmentTypeId = input.shipmentTypeId,

                    BranchSrcId = (await _branch.GetNearestByLocation(
                        new Dtos.Location
                        {
                            Latitude = input.senderLat,
                            Longitude = input.senderLong
                        })).Id,
                    BranchDstId = (await _branch.GetNearestByLocation(
                        new Dtos.Location
                        {
                            Latitude = input.receiverLat,
                            Longitude = input.receiverLong
                        })).Id,

                    TransactionToken = input.token,
                };

                var result = await _shipment.Insert(shipmentObj);

                return Ok(
                    new ReturnSuccessDto<DtoShipmentCreateReturn>
                    {
                        data = new DtoShipmentCreateReturn
                        {
                            shipmentId = result.Id,
                            statusOrder = result.Status.ToString()
                        }
                    }
                );
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(
                    new ReturnErrorDto
                    {
                        message = ex.Message
                    }
                );
            }
        }

        [HttpPatch("{id}/Status/{status}")]
        public async Task<ActionResult<ReturnSuccessDto<DtoStatus>>> UpdateShipmentStatus(int id, Status status)
        {
            try
            {
                var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(
                    Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "")
                );

                var userRole = decodedToken.Claims.Where(
                    claim => claim.Type.Equals("role")
                ).First();

                var shipment = await _shipment.GetById(id);

                if (userRole.Value.Equals("Branch"))
                {
                    // TODO Create a method for validating ID of JWT Token
                    var userId = decodedToken.Claims.Where(
                        claim => claim.Type.Equals("UserId")
                    ).First();

                    var branch = await _branch.GetByUserId(userId.Value);

                    if (status == Status.SendingToDestBranch && shipment.BranchSrcId != branch.Id)
                        return Forbid("Bearer");

                    if (status == Status.ArrivedAtDestBranch && shipment.BranchDstId != branch.Id)
                        return Forbid("Bearer");

                    if (status == Status.Delivered && shipment.BranchDstId != branch.Id)
                        return Forbid("Bearer");
                }

                var result = await _shipment.Update(
                    shipment.Id,
                    new Shipment
                    {
                        Status = status,
                    }
                );

                if (status == Status.Delivered && result.TransactionType == TransactionType.Tokopodia)
                {
                    await _tokopodia.TransactionUpdateStatus(shipment.TransactionId, shipment.TransactionToken);

                    var userToken = await _http.LoginUser(
                        new LoginUserInput
                        {
                            // TODO move credentials to appsetting?
                            Username = "Dianter",
                            Password = "@Pass123",
                        }
                    );

                    await _http.UpdateStatusTransaction(shipment.TransactionId, userToken.Token);
                }

                return Ok(
                    new ReturnSuccessDto<DtoStatus>
                    {
                        data = new DtoStatus
                        {
                            Id = shipment.Id,
                            Status = result.Status.ToString(),
                        }
                    }
                );
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(
                    new ReturnErrorDto
                    {
                        message = ex.Message
                    }
                );
            }
        }
    }
}