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
            try
            {
                var result = await _branch.GetById(id);
                return Ok(_mapper.Map<BranchDto>(result));
            }
            catch (Exception ex)
            {     
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Put(int id, [FromBody] BranchCreateDto branchCreateDto)
        {
            try
            {
                var branch = _mapper.Map<Branch>(branchCreateDto);
                var result = await _branch.Update(id, branch);
                return Ok("Data branch berhasil di update");
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
                await _branch.DeleteById(id);
                return Ok($"Data branch {id} berhasil didelete");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }  

    }
}