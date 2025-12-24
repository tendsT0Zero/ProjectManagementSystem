namespace ProjectManagementSystem.API.Models.DTOs
{
    public class ResponseDto
    {
        public object? ResponseObject { get; set; }
        public bool IsSuccess { get; set; } =false;
        public string? ErrorMessage { get; set; }
    }
}
