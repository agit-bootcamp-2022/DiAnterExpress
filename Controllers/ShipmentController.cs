using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Dtos;
using DiAnterExpress.External.SyncDataService.Http;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace DiAnterExpress.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentFeeClient _client;

        public ShipmentController(IShipmentFeeClient client)
        {
            _client = client;
        }

        [HttpPost]
        public async Task<ActionResult<ShipmentFeeOutput>> GetShipmentFee(ShipmentFeeInput input)
        {
            try
            {
                var shipmentType = await _client.GetFee(input.ShipmentTypeId);
                if (shipmentType != null)
                {
                    var senderLoc = new Point(input.SenderAddress.Latitude, input.SenderAddress.Longitude) { SRID = 4326 };
                    var receiverLoc = new Point(input.ReceiverAddress.Latitude, input.ReceiverAddress.Longitude) { SRID = 4326 };
                    var distance = senderLoc.Distance(receiverLoc) / 1000;
                    var fee = (distance * shipmentType.CostPerKm) + (input.Weight * shipmentType.CostPerKg);
                    return Ok(new ShipmentFeeOutput
                    {
                        Status = "Success",
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