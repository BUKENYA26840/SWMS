using Microsoft.AspNetCore.Mvc;
using Practical_Assignment.DTOs;
using Practical_Assignment.Services.Interfaces;

namespace Practical_Assignment.Controllers
{
    [Route("api/wallet")]
    public class WalletController : BaseController
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("balance")]
        public IActionResult GetBalance()
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null)
            {
                return Unauthorized(new ApiResponse<object> { Status = "error", Message = "Unauthorized" });
            }

            var response = _walletService.GetBalance(studentId);
            if (response.Status == "error")
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null)
            {
                return Unauthorized(new ApiResponse<object> { Status = "error", Message = "Unauthorized" });
            }

            var response = _walletService.GetProfile(studentId);
            if (response.Status == "error")
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
