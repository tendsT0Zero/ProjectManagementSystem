namespace ProjectManagementSystem.API.Static_Details
{
    public static class SD
    {
        public enum UserRoleType
        {
            ProjectManager,
            TeamLeader,
            TeamMember
        }

        public enum TaskStatus
        {
            Pending,
            InProgress,
            Completed,
            OnHold
        }

        public enum  TaskPriority
        {
            Medium,
            High,
            Low
        }
    }
}
