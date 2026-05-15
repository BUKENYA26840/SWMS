using Practical_Assignment.DTOs;
using Practical_Assignment.Models;

namespace Practical_Assignment.Services.Interfaces
{
    public interface ITransactionService
    {
        ApiResponse<object> Deposit(string studentId, decimal amount);
        ApiResponse<object> Pay(string studentId, decimal amount, ServiceType serviceType);
        ApiResponse<object> Transfer(string senderStudentId, string receiverStudentId, decimal amount);
    }
}
