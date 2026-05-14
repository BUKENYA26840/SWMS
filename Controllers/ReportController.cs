using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practical_Assignment.Data;
using Practical_Assignment.DTOs;
using System.Text;

namespace Practical_Assignment.Controllers
{
    [Route("api/report")]
    public class ReportController : BaseController
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("history/{studentId}")]
        public IActionResult GetHistory(string studentId)
        {
            // Note: Ideally, only the student themselves or an admin should view this.
            // Assuming no admin role here, we could optionally restrict to the logged-in user.
            // The prompt says "All transactions for student", so we'll just return it.
            var tokenStudentId = GetStudentIdFromToken();
            if (tokenStudentId == null || tokenStudentId != studentId)
            {
                // Basic auth check: can only view own history
                return Unauthorized(new ApiResponse<object> { Status = "error", Message = "Unauthorized or can only view your own history" });
            }

            var wallet = _context.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefault(w => w.StudentId == studentId);

            if (wallet == null)
            {
                return NotFound(new ApiResponse<object> { Status = "error", Message = "Wallet not found" });
            }

            var transactions = wallet.Transactions.OrderByDescending(t => t.Timestamp).ToList();

            return Ok(new ApiResponse<object>
            {
                Status = "success",
                Message = "History retrieved successfully",
                Data = transactions
            });
        }

        [HttpGet("daily-summary")]
        public IActionResult GetDailySummary()
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null)
            {
                return Unauthorized(new ApiResponse<object> { Status = "error", Message = "Unauthorized" });
            }

            var wallet = _context.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefault(w => w.StudentId == studentId);

            if (wallet == null)
            {
                return NotFound(new ApiResponse<object> { Status = "error", Message = "Wallet not found" });
            }

            var today = DateTime.UtcNow.Date;
            var todayTransactions = wallet.Transactions
                .Where(t => t.Timestamp.Date == today)
                .OrderByDescending(t => t.Timestamp)
                .ToList();

            return Ok(new ApiResponse<object>
            {
                Status = "success",
                Message = "Daily summary retrieved successfully",
                Data = todayTransactions
            });
        }

        [HttpGet("summary")]
        public IActionResult GetSummary()
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null)
            {
                return Unauthorized(new ApiResponse<object> { Status = "error", Message = "Unauthorized" });
            }

            var wallet = _context.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefault(w => w.StudentId == studentId);

            if (wallet == null)
            {
                return NotFound(new ApiResponse<object> { Status = "error", Message = "Wallet not found" });
            }

            var totalDeposits = wallet.Transactions
                .Where(t => t.Type == "DEPOSIT" && t.IsSuccess)
                .Sum(t => t.Amount);

            var totalPayments = wallet.Transactions
                .Where(t => t.Type == "WITHDRAW" && t.IsSuccess)
                .Sum(t => t.Amount);

            return Ok(new ApiResponse<object>
            {
                Status = "success",
                Message = "Summary retrieved successfully",
                Data = new
                {
                    TotalDeposits = totalDeposits,
                    TotalPayments = totalPayments
                }
            });
        }

        [HttpGet("export-csv")]
        public IActionResult ExportCsv()
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null)
            {
                return Unauthorized(new ApiResponse<object> { Status = "error", Message = "Unauthorized" });
            }

            var wallet = _context.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefault(w => w.StudentId == studentId);

            if (wallet == null)
            {
                return NotFound(new ApiResponse<object> { Status = "error", Message = "Wallet not found" });
            }

            var transactions = wallet.Transactions.OrderByDescending(t => t.Timestamp).ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Date,Type,Amount,Description,Status");

            foreach (var tx in transactions)
            {
                csv.AppendLine($"{tx.Timestamp:yyyy-MM-dd HH:mm:ss},{tx.Type},{tx.Amount},\"{tx.Description}\",{(tx.IsSuccess ? "Success" : "Failed")}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"transactions_{studentId}.csv");
        }
    }
}
