using Practical_Assignment.Models;

namespace Practical_Assignment.DTOs
{
    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public ServiceType ServiceType { get; set; }
    }
}
