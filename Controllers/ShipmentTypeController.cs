using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private IMapper _mapper;

        public ShipmentTypeController(IShipmentType shipmentType, IMapper mapper)
        {
            _shipmentType = shipmentType;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DtoShipmentType>>> Get()
        {
            try
            {
                var response = await _shipmentType.GetAll();
                return Ok(_mapper.Map<IEnumerable<DtoShipmentType>>(response));

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DtoShipmentType>> GetById(int id)
        {
            try
            {
                var data = await _shipmentType.GetById(id);
                var response = _mapper.Map<DtoShipmentType>(data);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<DtoShipmentType>> Post([FromBody] CreateShipmentType obj)
        {
            try
            {
                var map = _mapper.Map<ShipmentType>(obj);
                var data = await _shipmentType.Insert(map);
                var response = _mapper.Map<DtoShipmentType>(data);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DtoShipmentType>> Put(int id, [FromBody] CreateShipmentType obj)
        {
            try
            {
                var map = _mapper.Map<ShipmentType>(obj);
                var data = await _shipmentType.Update(id, map);
                var response = _mapper.Map<DtoShipmentType>(data);
                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
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

                return BadRequest(ex.Message);
            }
        }

    }
}