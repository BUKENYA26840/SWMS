namespace Practical_Assignment.DTOs
{
    public class TransferRequest
    {
        public string ReceiverStudentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
