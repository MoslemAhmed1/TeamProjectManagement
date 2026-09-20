namespace TeamProjectManagement.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public List<Project> OwnedProjects { get; set; } = new List<Project>();
        public List<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
        public List<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();

    }
}
