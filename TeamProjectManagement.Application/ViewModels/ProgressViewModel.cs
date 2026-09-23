namespace TeamProjectManagement.Application.ViewModels
{
    public record ProgressViewModel
    {
        public Guid ProjectId { get; set; }
        public double ProgressPercent { get; set; }
    }
}
