using Microsoft.AspNetCore.Mvc;
using Practical_Assignment.DTOs;
using Practical_Assignment.Services.Interfaces;

namespace Practical_Assignment.Controllers
{
    [Route("api/transaction")]
    public class TransactionController : BaseController
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("deposit")]
        public IActionResult Deposit([FromBody] DepositRequest request)
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null)
            {
                return Unauthorized(new ApiResponse<object> { Status = "error", Message = "Unauthorized" });
            }

            var response = _transactionService.Deposit(studentId, request.Amount);
            if (response.Status == "error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("pay")]
        public IActionResult Pay([FromBody] PaymentRequest request)
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null)
            {
                return Unauthorized(new ApiResponse<object> { Status = "error", Message = "Unauthorized" });
            }

            var response = _transactionService.Pay(studentId, request.Amount, request.ServiceType);
            if (response.Status == "error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("transfer")]
        public IActionResult Transfer([FromBody] TransferRequest request)
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null)
            {
                return Unauthorized(new ApiResponse<object> { Status = "error", Message = "Unauthorized" });
            }

            var response = _transactionService.Transfer(studentId, request.ReceiverStudentId, request.Amount);
            if (response.Status == "error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
