using Microsoft.AspNetCore.Mvc;
using TodoApi.AuthenticationService.Interfaces;
using TodoApi.AuthenticationService.Models;

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
                if (ex.Message.Contains("Username is already taken"))
                {
                    return Conflict($"The username '{model.Username}' is already taken.");
                }
                else if (ex.Message.Contains("Email is already taken"))
                {
                    return Conflict($"The email '{model.Email}' is already taken.");
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while registering the user.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            var token = await _userService.AuthenticateAsync(model.Email, model.Password);

            if (!string.IsNullOrEmpty(token))
            {
                return Ok(new { Token = token });
            }
            else
            {
                return BadRequest("Invalid email or password");
            }
        }

    }
}