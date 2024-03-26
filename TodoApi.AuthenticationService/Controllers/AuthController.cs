using Microsoft.AspNetCore.Mvc;
using TodoApi.AuthenticationService.Interfaces;
using TodoApi.AuthenticationService.Models;
using TodoApi.AuthenticationService.Services;

namespace TodoApi.AuthenticationService.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _userService.RegisterAsync(model.Username, model.Email, model.Password);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                // Handle the case where the email is already taken
                return Conflict($"The email '{model.Email}' is already taken.");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, "An error occurred while registering the user.");
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            var token = await _userService.AuthenticateAsync(model.Username, model.Password);

            return Ok(new { Token = token });
        }
    }
}
