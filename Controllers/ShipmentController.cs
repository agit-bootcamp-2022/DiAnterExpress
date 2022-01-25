using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Data;
using DiAnterExpress.Dtos;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace DiAnterExpress.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentType _shipmentType;

        public ShipmentController(IShipmentType shipmentType)
        {
            _shipmentType = shipmentType;
        }

        [HttpPost]
        public async Task<ActionResult<ShipmentFeeOutput>> GetShipmentFee(ShipmentFeeInput input)
        {
            try
            {
                var shipmentType = await _shipmentType.GetById(input.ShipmentTypeId);
                if (shipmentType != null)
                {
                    var senderLoc = new Point(input.SenderAddress.Latitude, input.SenderAddress.Longitude) { SRID = 4326 };
                    var receiverLoc = new Point(input.ReceiverAddress.Latitude, input.ReceiverAddress.Longitude) { SRID = 4326 };
                    var distance = senderLoc.Distance(receiverLoc) / 1000;
                    var fee = (distance * shipmentType.CostPerKm) + (input.Weight * shipmentType.CostPerKg);
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
    }
}