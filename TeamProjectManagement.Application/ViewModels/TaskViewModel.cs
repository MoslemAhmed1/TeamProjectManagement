namespace TeamProjectManagement.Application.ViewModels
{
    public record TaskViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Priority { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime? DueAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? AssignedToId { get; set; }
        public string? AssignedToUsername { get; set; }
    }
}
