using Practical_Assignment.DTOs;

namespace Practical_Assignment.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> LoginAsync(LoginRequest request);
        Task<ApiResponse<object>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<object>> LogoutAsync(string token);
    }
}
