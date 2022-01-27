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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using NetTopologySuite.Geometries;
using DiAnterExpress.Externals.Dtos;

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
        private readonly IUangTransDataClient _uangTrans;
        private readonly ITransactionInternal _transactionInternal;
        private readonly IBranch _branch;
        private readonly ITokopodiaDataClient _tokopodia;

        public ShipmentController(
            IShipmentType shipmentType,
            IShipment shipment,
            IMapper mapper,
            IUangTransDataClient uangTrans,
            ITransactionInternal transactionInternal,
            IBranch branch,
            ITokopodiaDataClient tokopodia
        )
        {
            _shipmentType = shipmentType;
            _shipment = shipment;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _uangTrans = uangTrans;
            _transactionInternal = transactionInternal;
            _branch = branch;
            _tokopodia = tokopodia;
        }

        [AllowAnonymous]
        [HttpPost("fee")]
        public async Task<ActionResult<ReturnSuccessDto<ShipmentFeeOutputDto>>> GetShipmentFee(ShipmentFeeInsertDto input)
        {
            try
            {
                var shipmentType = await _shipmentType.GetById(input.ShipmentTypeId);
                var fee = await _shipment.GetShipmentFee(input, shipmentType.CostPerKm, shipmentType.CostPerKg);
                return Ok(
                    new ReturnSuccessDto<ShipmentFeeOutputDto>
                    {
                        data = new ShipmentFeeOutputDto
                        {
                            Fee = fee
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

        [AllowAnonymous]
        [HttpPost("fee/all")]
        public async Task<ActionResult<ReturnSuccessDto<IEnumerable<ShipmentFeeAllTypeOutputDto>>>> GetShipmentFeeAllType(ShipmentFeeAllTypeInsertDto input)
        {
            try
            {
                var shipmentTypes = await _shipmentType.GetAll();
                if (!shipmentTypes.Any()) return NotFound("ShipmentType kosong");

                var newInput = _mapper.Map<ShipmentFeeInsertDto>(input);
                var response = new List<ShipmentFeeAllTypeOutputDto>();

                foreach (var shipmentType in shipmentTypes)
                {
                    var fee = await _shipment.GetShipmentFee(newInput, shipmentType.CostPerKm, shipmentType.CostPerKg);
                    response.Add(
                        new ShipmentFeeAllTypeOutputDto
                        {
                            Id = shipmentType.Id,
                            Name = shipmentType.Name,
                            TotalFee = fee
                        }
                    );
                }
                return Ok(
                    new ReturnSuccessDto<IEnumerable<ShipmentFeeAllTypeOutputDto>>
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
        public async Task<ActionResult<ReturnSuccessDto<ShipmentInternalOutputDto>>> CreateShipmentInternal(ShipmentInternalInsertDto input)
        {
            try
            {
                var shipmentType = await _shipmentType.GetById(input.ShipmentTypeId);

                var mapCost = new ShipmentFeeInsertDto
                {
                    SenderLat = input.SenderLocation.Latitude,
                    SenderLong = input.SenderLocation.Longitude,
                    ReceiverLat = input.ReceiverLocation.Latitude,
                    ReceiverLong = input.ReceiverLocation.Longitude,
                    Weight = input.TotalWeight,
                    ShipmentTypeId = input.ShipmentTypeId
                };

                var fee = await _shipment.GetShipmentFee(mapCost, shipmentType.CostPerKm, shipmentType.CostPerKg);
                var inputHttp = new TransferBalanceDto
                {
                    customerDebitId = input.UangTransUserId,
                    Amount = fee,
                    customerCreditId = 1 //TODO Replace customerCreditId with AnterAjaUangTransId(?)
                };

                var httpRequest = await _http.CreateShipmentInternal(inputHttp);
                if (httpRequest.Succeed)
                {
                    var transactionInternal = new TransactionInternal
                    {
                        Product = input.Product,
                        PaymmentId = 0 //TODO Replace paymentId from UangTrans
                    };
                    var fee = await _shipment.GetShipmentFee(mapCost, shipmentType.CostPerKm, shipmentType.CostPerKg);

                    var loginUser = new LoginUserInput
                    {
                        Username = input.UserCredential.UangTransUsername,
                        Password = input.UserCredential.UangTransPassword
                    };
                    var token = await _uangTrans.LoginUser(loginUser);
                    var customerId = await _uangTrans.GetProfile(token.Token);

                    var inputHttp = new TransferBalanceDto
                    {
                        CustomerDebitId = customerId.Id,
                        Amount = fee,
                        CustomerCreditId = 5 //5 is AnterAja Id in UangTrans Service
                    };

                    var httpRequest = await _uangTrans.CreateShipmentInternal(inputHttp, token.Token);
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
                            new ReturnSuccessDto<ShipmentInternalOutputDto>
                            {
                                data = new ShipmentInternalOutputDto
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
        public async Task<ActionResult<ReturnSuccessDto<IEnumerable<ShipmentOutputDto>>>> GetAllShipment()
        {
            var result = await _shipment.GetAll();
            var dtos = _mapper.Map<IEnumerable<ShipmentOutputDto>>(result);
            return Ok(
                new ReturnSuccessDto<IEnumerable<ShipmentOutputDto>>
                {
                    data = dtos
                }
            );
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReturnSuccessDto<ShipmentOutputDto>>> GetShipmentById(int id)
        {
            try
            {
                var result = await _shipment.GetById(id);

                return Ok(
                    new ReturnSuccessDto<ShipmentOutputDto>
                    {
                        data = _mapper.Map<ShipmentOutputDto>(result)
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

        [AllowAnonymous]
        [HttpGet("{id}/Status")]
        public async Task<ActionResult<ReturnSuccessDto<StatusOutputDto>>> GetShipmentStatus(int id)
        {
            try
            {
                var result = await _shipment.GetById(id);

                return Ok(
                    new ReturnSuccessDto<StatusOutputDto>
                    {
                        data = _mapper.Map<StatusOutputDto>(result)
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

        [Authorize(Roles = "Admin, Tokopodia")]
        [HttpPost("tokopodia")]
        public async Task<ActionResult<ReturnSuccessDto<DtoShipmentCreateReturn>>> CreateShipmentTokpod([FromBody] DtoShipmentCreateTokopodia input)
        {
            try
            {
                var shipmentType = await _shipmentType.GetById(input.shipmentTypeId);

                var totalFee = await _shipment.GetShipmentFee(
                    new ShipmentFeeInsertDto
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
        public async Task<ActionResult<ReturnSuccessDto<StatusOutputDto>>> UpdateShipmentStatus(int id, Status status)
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

                    var userToken = await _uangTrans.LoginUser(
                        new LoginUserInput
                        {
                            // TODO move credentials to appsetting?
                            Username = "Dianter",
                            Password = "@Pass123",
                        }
                    );

                    await _uangTrans.UpdateStatusTransaction(shipment.TransactionId, userToken.Token);
                }

                return Ok(
                    new ReturnSuccessDto<StatusOutputDto>
                    {
                        data = new StatusOutputDto
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