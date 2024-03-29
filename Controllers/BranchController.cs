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
        public async Task<ActionResult<ReturnSuccessDto<IEnumerable<BranchOutputDto>>>> GetAll()
        {
            var branches = await _branch.GetAll();
            var dtos = _mapper.Map<IEnumerable<BranchOutputDto>>(branches);
            return Ok(
                new ReturnSuccessDto<IEnumerable<BranchOutputDto>>
                {
                    data = dtos
                }
            );
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ReturnSuccessDto<BranchOutputDto>>> GetBranchById(int id)
        {
            try
            {
                var result = await _branch.GetById(id);
                return Ok(
                    new ReturnSuccessDto<BranchOutputDto>
                    {
                        data = _mapper.Map<BranchOutputDto>(result)
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
        public async Task<ActionResult<ReturnSuccessDto<string>>> UpdateBranch(int id, [FromBody] BranchInsertDto input)
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

                var branch = _mapper.Map<Branch>(input);
                var result = await _branch.Update(id, branch);
                return Ok(
                    new ReturnSuccessDto<string>
                    {
                        data = "Data branch berhasil di update"
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

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ReturnSuccessDto<string>>> Delete(int id)
        {
            try
            {
                await _branch.Delete(id);
                return Ok(
                    new ReturnSuccessDto<string>
                    {
                        data = $"Data branch {id} berhasil didelete"
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