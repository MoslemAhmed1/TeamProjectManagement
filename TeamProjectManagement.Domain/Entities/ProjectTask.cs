using TeamProjectManagement.Domain.Enums;

namespace TeamProjectManagement.Domain.Entities
{
    public class ProjectTask : IHasCreatedAt, IHasUpdatedAt
    {
        private string _title = null!;
        private string? _description;

        public Guid Id { get; set; }

        public string Title
        {
            get => _title;
            set => _title = Text.Required(value);
        }

        public string? Description
        {
            get => _description;
            set => _description = Text.Optional(value);
        }

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
