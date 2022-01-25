using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Data;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiAnterExpress.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShipmentTypeController : ControllerBase
    {
        private IShipmentType _shipmentType;

        public ShipmentTypeController(IShipmentType shipmentType)
        {
            _shipmentType = shipmentType;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipmentType>>> Get()
        {
            try
            {
                var response = await _shipmentType.GetAll();
                return Ok(response);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShipmentType>> GetByIdCustomer(int id)
        {
            try
            {
                var response = await _shipmentType.GetById(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Messege = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ShipmentType>> Post([FromBody] CreateShipmentType obj)
        {
            try
            {
                var map = new ShipmentType
                {
                    Name = obj.Name,
                    CostPerKg = obj.CostPerKg,
                    CostPerKm = obj.CostPerKm,
                };
                var response = await _shipmentType.Insert(map);
                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest(new { Status = 400, Messege = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CreateShipmentType obj)
        {
            try
            {
                var map = new ShipmentType
                {
                    Name = obj.Name,
                    CostPerKg = obj.CostPerKg,
                    CostPerKm = obj.CostPerKm,
                };
                var response = await _shipmentType.Update(id, map);
                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest(new { Status = 400, Messege = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var response = await _shipmentType.Delete(id);
                return Ok(new { Message = "Success delete data" });
            }
            catch (Exception ex)
            {

                return BadRequest(new { Status = 400, Messege = ex.Message });
            }
        }

    }
}