namespace TeamProjectManagement.Domain.Entities
{
    public class User
    {
        Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public List<Project> Projects { get; set; } = new List<Project>();
        public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }
}
