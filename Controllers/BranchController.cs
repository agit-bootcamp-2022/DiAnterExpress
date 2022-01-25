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
    public class BranchController : ControllerBase
    {
        private IBranch _branch;
        private IMapper _mapper;
        public BranchController(IBranch branch, IMapper mapper)
        {
            _branch = branch ?? throw new ArgumentNullException(nameof(branch));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BranchDto>>> GetAll()
        {
            var drivers = await _branch.GetAll();
            var dtos = _mapper.Map<IEnumerable<BranchDto>>(drivers);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BranchDto>> GetBranchById(int id)
        {
            var result = await _branch.GetById(id);
            if (result == null)
                return NotFound();

            return Ok(_mapper.Map<BranchDto>(result));
        }

        [HttpPost]
        public async Task<ActionResult<BranchDto>> Post([FromBody] BranchCreateDto branchCreateDto)
        {
           try
            {
                var result = await _branch.Insert(_mapper.Map<Branch>(branchCreateDto));

                return Ok(_mapper.Map<BranchDto>(result));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BranchDto>> Put(int id, [FromBody] BranchCreateDto branchCreateDto)
        {
            try
            {
                var branch = _mapper.Map<Branch>(branchCreateDto);
                var result = await _branch.Update(id, branch);
                var branchdto = _mapper.Map<Dtos.BranchDto>(result);
                return Ok(branchdto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _branch.Delete(id);
                return Ok($"Data student {id} berhasil didelete");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }  
        
    }
}