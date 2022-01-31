using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DiAnterExpress.Data;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiAnterExpress.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/v1/Shipment/Type")]
    public class ShipmentTypeController : ControllerBase
    {
        private IShipmentType _shipmentType;
        private IMapper _mapper;

        public ShipmentTypeController(IShipmentType shipmentType, IMapper mapper)
        {
            _shipmentType = shipmentType;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<ReturnSuccessDto<IEnumerable<DtoShipmentType>>>> Get()
        {
            try
            {
                var response = await _shipmentType.GetAll();
                return Ok(
                    new ReturnSuccessDto<IEnumerable<DtoShipmentType>>
                    {
                        data = _mapper.Map<IEnumerable<DtoShipmentType>>(response)
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

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ReturnSuccessDto<DtoShipmentType>>> GetById(int id)
        {
            try
            {
                var data = await _shipmentType.GetById(id);
                var response = _mapper.Map<DtoShipmentType>(data);
                return Ok(
                    new ReturnSuccessDto<DtoShipmentType>
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

        [HttpPost]
        public async Task<ActionResult<ReturnSuccessDto<DtoShipmentType>>> Post([FromBody] ShipmentTypeInsertDto obj)
        {
            try
            {
                var map = _mapper.Map<ShipmentType>(obj);
                var data = await _shipmentType.Insert(map);
                var response = _mapper.Map<DtoShipmentType>(data);
                return Ok(
                    new ReturnSuccessDto<DtoShipmentType>
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

        [HttpPut("{id}")]
        public async Task<ActionResult<ReturnSuccessDto<DtoShipmentType>>> Put(int id, [FromBody] ShipmentTypeInsertDto obj)
        {
            try
            {
                var map = _mapper.Map<ShipmentType>(obj);
                var data = await _shipmentType.Update(id, map);
                var response = _mapper.Map<DtoShipmentType>(data);
                return Ok(
                    new ReturnSuccessDto<DtoShipmentType>
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<ReturnSuccessDto<string>>> Delete(int id)
        {
            try
            {
                var response = await _shipmentType.Delete(id);
                return Ok(
                    new ReturnSuccessDto<string>
                    {
                        data = "Berhasil delete data"
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

    }
}