namespace TeamProjectManagement.Application.ViewModels
{
    public record ProjectDetailsViewModel : ProjectViewModel
    {
        public List<MemberViewModel> Members { get; set; } = new();
        public List<TaskViewModel> Tasks { get; set; } = new();
    }
}
