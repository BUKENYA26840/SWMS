using System.Text.Json.Serialization;

namespace Practical_Assignment.Models
{
    public class Student
    {
        public string StudentId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PIN { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
        public int FailedAttempts { get; set; }
        
        [JsonIgnore]
        public Wallet? Wallet { get; set; }
    }
}
