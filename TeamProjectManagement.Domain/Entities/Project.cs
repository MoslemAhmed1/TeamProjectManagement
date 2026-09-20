namespace TeamProjectManagement.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        public List<ProjectMember> Members { get; set; } = new List<ProjectMember>();
        public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }
}

