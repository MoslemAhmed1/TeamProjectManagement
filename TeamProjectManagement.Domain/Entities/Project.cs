namespace TeamProjectManagement.Domain.Entities
{
    public class Project : IHasCreatedAt, IHasUpdatedAt
    {
        private string _name = null!;
        private string? _description;

        public Guid Id { get; set; }

        public string Name
        {
            get => _name;
            set => _name = Text.Required(value);
        }

        public string? Description
        {
            get => _description;
            set => _description = Text.Optional(value);
        }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        public List<ProjectMember> Members { get; set; } = new List<ProjectMember>();
        public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }
}
