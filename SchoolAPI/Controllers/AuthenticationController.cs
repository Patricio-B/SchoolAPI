using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Contracts;
using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace SchoolAPI.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<Student> _studentManager;
        private readonly IAuthenticationManager _authManager;

        public AuthenticationController(ILoggerManager logger, IMapper mapper, UserManager<Student> studentManager, IAuthenticationManager authManager)
        {
            _logger = logger;
            _mapper = mapper;
            _studentManager = studentManager;
            _authManager = authManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] StudentForAuthenticationDto student)
        {
            if (!await _authManager.ValidateStudent(student))
            {
                _logger.LogWarn($"{nameof(Authenticate)}: Authentication failed. Wrong user name or password.");
                return Unauthorized();
            }

            return Ok(new { Token = await _authManager.CreateToken() });
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] StudentForRegistrationDto studentForRegistration)
        {
            var student = _mapper.Map<Student>(studentForRegistration);

            var result = await _studentManager.CreateAsync(student, studentForRegistration.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            await _studentManager.AddToRolesAsync(student, studentForRegistration.Roles);

            return StatusCode(201);
        }

        
    }
}