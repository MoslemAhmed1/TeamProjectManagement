namespace TeamProjectManagement.Application.ViewModels
{
    public record ProjectViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerUsername { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public double ProgressPercent { get; set; }
    }
}
