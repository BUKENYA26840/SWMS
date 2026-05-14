namespace Practical_Assignment.DTOs
{
    public class ApiResponse<T>
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
