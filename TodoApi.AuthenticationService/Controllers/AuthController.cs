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
                // Check if the exception message contains "Username is already taken"
                if (ex.Message.Contains("Username is already taken"))
                {
                    return Conflict($"The username '{model.Username}' is already taken.");
                }
                // Check if the exception message contains "Email is already taken"
                else if (ex.Message.Contains("Email is already taken"))
                {
                    return Conflict($"The email '{model.Email}' is already taken.");
                }
                // Handle other ArgumentExceptions
                else
                {
                    return BadRequest(ex.Message);
                }
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
            // Authenticate user
            var token = await _userService.AuthenticateAsync(model.Username, model.Password);

            // Check if authentication was successful
            if (!string.IsNullOrEmpty(token))
            {
                // Return token if authentication was successful
                return Ok(new { Token = token });
            }
            else
            {
                // Return BadRequest with appropriate error message for invalid login credentials
                return BadRequest("Invalid email or password");
            }
        }

    }
}