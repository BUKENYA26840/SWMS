using Microsoft.AspNetCore.Mvc;
using Practical_Assignment.Services;

namespace Practical_Assignment.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected string? GetStudentIdFromToken()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            var token = authHeader?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();
            
            if (string.IsNullOrEmpty(token))
            {
                token = Request.Query["token"].FirstOrDefault();
            }

            if (string.IsNullOrEmpty(token)) return null;
            return AuthService.GetStudentIdFromToken(token);
        }
    }
}
