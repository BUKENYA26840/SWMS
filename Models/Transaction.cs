using System.Text.Json.Serialization;

namespace Practical_Assignment.Models
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public Guid WalletId { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ReceiverStudentId { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsSuccess { get; set; }
        
        [JsonIgnore]
        public Wallet? Wallet { get; set; }
    }
}
