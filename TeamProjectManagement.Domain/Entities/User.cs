namespace TeamProjectManagement.Domain.Entities
{
    public class User : IHasCreatedAt
    {
        private string _username = null!;
        private string _email = null!;

        public Guid Id { get; set; }

        public string Username
        {
            get => _username;
            set => _username = Text.Identity(value);
        }

        public string Email
        {
            get => _email;
            set => _email = Text.Identity(value);
        }

        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public List<Project> OwnedProjects { get; set; } = new List<Project>();
        public List<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
        public List<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
