namespace MyNewApp.Domain.DTOs
{
    public class ResponseDTO
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
        public object? Data { get; set; }
    }
}
