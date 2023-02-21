using System.Net;
using API.Helpers;
using Entity;
using Entity.DTOs;
using Entity.Entities;
using Entity.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class AccountController : BaseController
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {

            WebResponse<RegisterUserDto> response = new WebResponse<RegisterUserDto>();
            var result = await _accountRepository.RegisterUserAsync(registerUser);

            if (result == null)
            {
                response.StatusCode = 500;
                response.Message = "Failed to register";

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }


            response.StatusCode = 201;
            response.Message = "Register Success";
            response.Data = result;
            return StatusCode(StatusCodes.Status201Created, response);


        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
        {
            WebResponse<LoginUserDto> response = new WebResponse<LoginUserDto>();
            var result = await _accountRepository.LoginUserAsync(loginUser);
            
            if (result == null)
            {
                response.StatusCode = 401;
                response.Message = "Unauthorized";
                return StatusCode(StatusCodes.Status401Unauthorized, response);

            }
            response.StatusCode = 200;
            response.Message = "Login Success";
            response.Data = result;
            return StatusCode(StatusCodes.Status200OK, response);
        }
        
        

    }


}