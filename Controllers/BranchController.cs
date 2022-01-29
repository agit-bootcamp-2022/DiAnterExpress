using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DiAnterExpress.Data;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace DiAnterExpress.Controllers
{
    [Authorize]
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BranchDto>>> GetAll()
        {
            var drivers = await _branch.GetAll();
            var dtos = _mapper.Map<IEnumerable<BranchDto>>(drivers);
            return Ok(dtos);
        }

        [AllowAnonymous]
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
                var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(
                    Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "")
                );

                var userRole = decodedToken.Claims.Where(
                    claim => claim.Type.Equals("role")
                ).First();

                if (userRole.Value.Equals("Branch"))
                {
                    // TODO Create a method for validating ID of JWT Token
                    var userId = decodedToken.Claims.Where(
                        claim => claim.Type.Equals("UserId")
                    ).First();

                    var currentBranchId = await _branch.GetByUserId(userId.Value);

                    if (currentBranchId.Id != id) return Forbid("Bearer");
                }

                var branch = _mapper.Map<Branch>(branchCreateDto);
                var result = await _branch.Update(id, branch);
                return Ok("Data branch berhasil di update");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
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