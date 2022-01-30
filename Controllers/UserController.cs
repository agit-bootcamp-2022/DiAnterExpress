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
using NetTopologySuite.Geometries;

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
        public ActionResult<ReturnSuccessDto<IEnumerable<ApplicationUser>>> GetAllUser()
        {
            var result = _user.GetAllUser();
            return Ok(
                new ReturnSuccessDto<IEnumerable<ApplicationUser>>
                {
                    data = result
                }
            );
        }

        [HttpPost("Registration/Branch")]
        public async Task<ActionResult<ReturnSuccessDto<string>>> RegistrationBranch(DtoUserInputBranch input)
        {
            try
            {
                var result = await _user.Insert(_mapper.Map<DtoUserRegister>(input));
                var branch = new Branch
                {
                    Name = input.Name,
                    Address = input.Address,
                    City = input.City,
                    Phone = input.Phone,
                    Location = new Point(input.Longitude, input.Latitude) { SRID = 4326 },
                    UserId = result.Id
                };
                await _branch.Insert(branch);
                return Ok(
                    new ReturnSuccessDto<string>
                    {
                        data = "Berhasil register"
                    }
                );
            }
            catch (System.Exception ex)
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

        [HttpPost("Authentication")]
        public async Task<ActionResult<ReturnSuccessDto<string>>> Authentication([FromBody] DtoUserCredentials credentials)
        {
            try
            {
                var user = await _user.Authentication(credentials.Username, credentials.Password);

                return Ok(
                    new ReturnSuccessDto<string>
                    {
                        data = user
                    }
                );
            }
            catch (System.Exception ex)
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
        [HttpGet("servicetoken/{role}")]
        public ActionResult<ReturnSuccessDto<string>> GetServiceToken(string role)
        {
            try
            {
                return Ok(
                    new ReturnSuccessDto<string>
                    {
                        data = _user.GenerateServiceToken(role)
                    }
                );
            }
            catch (System.Exception ex)
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