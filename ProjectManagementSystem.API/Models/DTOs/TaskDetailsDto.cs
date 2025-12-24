namespace ProjectManagementSystem.API.Models.DTOs
{
    public class TaskDetailsDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string AssignedToUserName { get; set; }
        public string ProjectName { get; set; }
        public bool IsRequiredAttachment { get; set; }=false;
        public DateTime DueDate { get; set; }
    }
}
