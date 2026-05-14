using Microsoft.AspNetCore.Mvc;
using Practical_Assignment.DTOs;
using Practical_Assignment.Services.Interfaces;

namespace Practical_Assignment.Controllers
{
    [Route("api/auth")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (response.Status == "error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            if (response.Status == "error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized(new ApiResponse<object> { Status = "error", Message = "Missing token" });
            }

            var token = authHeader.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();
            var response = await _authService.LogoutAsync(token);

            if (response.Status == "error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
