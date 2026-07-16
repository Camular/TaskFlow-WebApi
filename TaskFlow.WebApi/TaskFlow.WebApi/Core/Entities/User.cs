namespace TaskFlow.WebApi.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; } 
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public ICollection<UserWorkspaceRole> UserWorkspaceRoles { get; set; } = new List<UserWorkspaceRole>();
        public ICollection<TaskItem> AssignedTaskItems { get; set; } = new List<TaskItem>();
    }
}
