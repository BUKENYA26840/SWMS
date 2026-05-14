using System.Text.Json.Serialization;

namespace Practical_Assignment.Models
{
    public class Wallet
    {
        public Guid WalletId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public Student? Student { get; set; }
        
        [JsonIgnore]
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
