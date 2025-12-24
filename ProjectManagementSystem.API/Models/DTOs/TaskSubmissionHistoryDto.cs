namespace ProjectManagementSystem.API.Models.DTOs
{
    public class TaskSubmissionHistoryDto
    {
        public string ProjectName { get; set; } = string.Empty;
        public string TaskTitle { get; set; } = string.Empty;
        public string TaskDescription { get; set; }

        public DateOnly SubmissionDate { get; set; }
        public string SubmitterName { get; set; } = string.Empty;
        public string SubmissionNotes { get; set; }
        public string SubmissionAttachmentUrl { get; set; }
    }
}
