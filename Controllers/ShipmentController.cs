using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DiAnterExpress.Data;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;
using DiAnterExpress.SyncDataServices.Http;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace DiAnterExpress.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentType _shipmentType;
        private readonly IShipment _shipment;
        private readonly IMapper _mapper;
        private readonly IShipmentInternalDataClient _http;
        private readonly ITransactionInternal _transactionInternal;

        public ShipmentController(IShipmentType shipmentType,
            IShipment shipment, IMapper mapper, IShipmentInternalDataClient http,
            ITransactionInternal transactionInternal)
        {
            _shipmentType = shipmentType;
            _shipment = shipment;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _http = http; 
            _transactionInternal = transactionInternal;
        }

        [HttpPost("fee")]
        public async Task<ActionResult<ShipmentFeeOutput>> GetShipmentFee(ShipmentFeeInput input)
        {
            try
            {
                var shipmentType = await _shipmentType.GetById(input.ShipmentTypeId);
                if (shipmentType != null)
                {
                    var fee = await _shipment.GetShipmentFee(input, shipmentType.CostPerKm, shipmentType.CostPerKg);
                    return Ok(new ShipmentFeeOutput
                    {
                        Fee = fee
                    });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("fee/all")]
        public async Task<ActionResult<IEnumerable<DtoShipmentFeeAllType>>> GetShipmentFeeAllType(ShipmentFeeAllInput input)
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
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("internal")]
        public async Task<ActionResult<ShipmentInternalOutput>> CreateShipmentInternal(ShipmentInternalInput input)
        {
            try
            {
                var shipmentType = await _shipmentType.GetById(input.ShipmentTypeId);
                if (shipmentType != null)
                {
                    var mapCost = new ShipmentFeeInput
                    {
                        SenderAddress = input.SenderLocation,
                        ReceiverAddress = input.ReceiverLocation,
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
                    if (httpRequest.Succeed == true)
                    {
                        var transactionInternal = new TransactionInternal
                        {
                            Product = input.Product,
                            PaymmentId = 0 //TODO Replace paymentId from UangTrans
                        };
                        var transactionId = await _transactionInternal.Insert(transactionInternal);

                        var shipment = new Shipment
                        {
                            SenderName = input.SenderName,
                            SenderContact = input.SenderContact,
                            SenderAddress = new Point(input.SenderLocation.Latitude, input.SenderLocation.Longitude) { SRID = 4326 },
                            ReceiverName = input.ReceiverName,
                            ReceiverContact = input.ReceiverContact,
                            ReceiverAddress = new Point(input.ReceiverLocation.Latitude, input.ReceiverLocation.Longitude) { SRID = 4326 },
                            TotalWeight = input.TotalWeight,
                            Cost = fee,
                            Status = Status.OrderReceived,
                            TransactionType = TransactionType.Internal,
                            TransactionId = transactionId.Id,
                            ShipmentTypeId = input.ShipmentTypeId,
                            BranchId = 1 //TODO Implement BranchId auto search func (?)
                        };
                        var shipmentResult = await _shipment.Insert(shipment);
                        return Ok(new ShipmentInternalOutput
                        {
                            ShipmentId = shipmentResult.Id,
                            StatusOrder = shipment.Status.ToString()
                        });
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DtoShipmentOutput>>> GetAllShipment()
        {
            var result = await _shipment.GetAll();
            var dtos = _mapper.Map<IEnumerable<DtoShipmentOutput>>(result);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DtoShipmentOutput>> GetShipmentById(int id)
        {
            var result = await _shipment.GetById(id);
            if (result == null)
                return NotFound();

            return Ok(_mapper.Map<DtoShipmentOutput>(result));
        }

        [HttpGet("{id}/Status")]
        public async Task<ActionResult<DtoStatus>> GetShipmentStatus(int id)
        {
            var result = await _shipment.GetById(id);
            if (result == null)
                return NotFound();

            return Ok(_mapper.Map<DtoStatus>(result));
        }
    }
}