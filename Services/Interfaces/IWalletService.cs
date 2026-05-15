using Practical_Assignment.DTOs;

namespace Practical_Assignment.Services.Interfaces
{
    public interface IWalletService
    {
        ApiResponse<decimal> GetBalance(string studentId);
        ApiResponse<object> GetProfile(string studentId);
    }
}
