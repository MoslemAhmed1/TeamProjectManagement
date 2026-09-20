using TeamProjectManagement.Domain.Enums;

namespace TeamProjectManagement.Domain.Entities
{
    public class ProjectTask
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DueAt { get; set; }
        public ProjectTaskPriority Priority { get; set; }
        public ProjectTaskStatus Status { get; set; }

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public Guid? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }
    }
}
