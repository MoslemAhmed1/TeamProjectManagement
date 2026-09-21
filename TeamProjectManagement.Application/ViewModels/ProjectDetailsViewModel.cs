namespace TeamProjectManagement.Application.ViewModels
{
    public class ProjectDetailsViewModel : ProjectViewModel
    {
        public double ProgressPercent { get; set; }
        public List<MemberViewModel> Members { get; set; } = new();
        public List<TaskViewModel> Tasks { get; set; } = new();
    }
}
