using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Data;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiAnterExpress.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private IUser _user;
        private IMapper _mapper;
        private IBranch _branch;

        public UserController(IUser user, IMapper mapper, IBranch branch)
        {
            _user = user;
            _mapper = mapper;
            _branch = branch;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<ApplicationUser> GetAllUser()
        {
            var result = _user.GetAllUser();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Registration/Branch")]
        public async Task<ActionResult<string>> RegistrationBranch(DtoUserInputBranch input)
        {
            try
            {
                var result = await _user.Insert(_mapper.Map<DtoUserRegister>(input));
                var branch = new Branch{
                            Name = input.Name,
                            Address = input.Address,
                            City = input.City,
                            Phone = input.Phone,
                            UserId = result.Id
                };
                await _branch.Insert(branch);
                return Ok("Berhasil register");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Authentication")]
        public async Task<ActionResult<ApplicationUser>> Authentication([FromBody] DtoUserCredentials credentials)
        {
            try
            {
                var user = await _user.Authentication(credentials.Username, credentials.Password);

                return Ok(user);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("servicetoken/{role}")]
        public ActionResult<string> GetServiceToken(string role)
        {
            try
            {
                return Ok(
                    _user.GenerateServiceToken(role)
                );
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}