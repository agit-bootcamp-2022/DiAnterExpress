using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IShipment _shipment;
        private readonly IMapper _mapper;

        public ShipmentController(IShipmentType shipmentType, 
            IShipment shipment, IMapper mapper)
        {
            _shipmentType = shipmentType;
            _shipment = shipment;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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