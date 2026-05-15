using Microsoft.EntityFrameworkCore;
using Practical_Assignment.Data;
using Practical_Assignment.DTOs;
using Practical_Assignment.Services.Interfaces;

namespace Practical_Assignment.Services
{
    public class WalletService : IWalletService
    {
        private readonly AppDbContext _context;

        public WalletService(AppDbContext context)
        {
            _context = context;
        }

        public ApiResponse<decimal> GetBalance(string studentId)
        {
            var wallet = _context.Wallets.FirstOrDefault(w => w.StudentId == studentId);
            if (wallet == null)
            {
                return new ApiResponse<decimal>
                {
                    Status = "error",
                    Message = "Wallet not found"
                };
            }

            return new ApiResponse<decimal>
            {
                Status = "success",
                Message = "Balance retrieved successfully",
                Data = wallet.Balance
            };
        }

        public ApiResponse<object> GetProfile(string studentId)
        {
            var student = _context.Students
                .Include(s => s.Wallet)
                .FirstOrDefault(s => s.StudentId == studentId);

            if (student == null)
            {
                return new ApiResponse<object>
                {
                    Status = "error",
                    Message = "Account not found"
                };
            }

            var profile = new
            {
                student.StudentId,
                student.FullName,
                student.IsLocked,
                Wallet = student.Wallet == null ? null : new
                {
                    student.Wallet.WalletId,
                    student.Wallet.Balance,
                    student.Wallet.CreatedAt
                }
            };

            return new ApiResponse<object>
            {
                Status = "success",
                Message = "Profile retrieved successfully",
                Data = profile
            };
        }
    }
}
