using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Data;
using DiAnterExpress.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiAnterExpress.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShipmentTypeController : ControllerBase
    {
       private readonly IShipmentType _shipmentType;

        public ShipmentTypeController(IShipmentType shipmentType)
        {
            _shipmentType = shipmentType;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShipmentType>> GetShipmentType(int id)
        {
            var result = await _shipmentType.GetById(id);
            return result;
        }
    }
}