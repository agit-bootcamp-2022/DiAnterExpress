using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DiAnterExpress.Data;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;
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
        private readonly ITransactionInternal _transactionInternal;
        private readonly IMapper _mapper;

        public ShipmentController(IShipmentType shipmentType,
            IShipment shipment, ITransactionInternal transactionInternal,
            IMapper mapper)
        {
            _shipmentType = shipmentType;
            _shipment = shipment;
            _transactionInternal = transactionInternal;
            _mapper = mapper;
        }

        [HttpPost("GetShipmentFee")]
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

        [HttpPost("CreateShipmentInternal")]
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
                    
                    //HTTP POST UangTransId & fee TO UANGTRANS
                    //IF (RETURN FROM UANG TRANS == POSITIF) THEN DO FOLLOWING

                    var transactionInternal = new TransactionInternal
                    {
                        Product = input.Product,
                        PaymmentId = 0 //nantinya bakal pakai paymentId yang direturn UangTrans
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
                        BranchId = 1
                    };
                    var shipmentResult = await _shipment.Insert(shipment);
                    return Ok(new ShipmentInternalOutput
                    {
                        ShipmentId = shipmentResult.Id,
                        StatusOrder = shipment.Status.ToString()
                    });

                    //ELSE -> return bad request

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
    }
}