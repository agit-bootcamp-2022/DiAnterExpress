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

        [HttpPost]
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

        [HttpPost]
        public async Task<ActionResult<ShipmentInternalOutput>> CreateShipmentInternal(ShipmentInternalInput input)
        {
            try
            {
                var mapCost = new ShipmentFeeInput  //map dulu biar getshipmentfee jalan
                {
                    SenderAddress = input.SenderAddress,
                    ReceiverAddress = input.ReceiverAddress,
                    Weight = input.TotalWeight,
                    ShipmentTypeId = input.ShipmentTypeId
                };
                var cost = await GetShipmentFee(mapCost); //get fee function
                //var return = Http Post ke UangTrans, kirim UangTransId Bersangkutan dan costnya
                // IF (return == positif)
                // {
                var transactionInternal = new TransactionInternal
                {
                    Product = input.Product,
                    PaymmentId = 0 //nantinya bakal pakai paymentId yang direturn UangTrans
                };
                var transactionId = await _transactionInternal.Insert(transactionInternal); //insert new transactionInternal

                var shipment = _mapper.Map<Shipment>(input);
                Console.WriteLine(shipment.ReceiverContact.ToString());
                Console.WriteLine(shipment.ReceiverAddress.ToString());
                shipment.Cost = cost.Value.Fee;
                shipment.Status = Status.OrderReceived; //Masih eksplisit, bisa masukin di profile mapper
                shipment.TransactionId = transactionId.Id;
                shipment.TransactionType = TransactionType.Internal;
                shipment.BranchId = 0;

                var shipmentResult = await _shipment.Insert(shipment);

                return Ok(new ShipmentInternalOutput
                {
                    ShipmentId = shipmentResult.Id,
                    StatusOrder = shipment.Status
                });
                // END IF }
                // else
                // {
                // return BadRequest();
                // }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}