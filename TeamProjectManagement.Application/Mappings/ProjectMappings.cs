using TeamProjectManagement.Application.ViewModels;
using TeamProjectManagement.Domain.Entities;
using TeamProjectManagement.Domain.Enums;

namespace TeamProjectManagement.Application.Mappings
{
    public static partial class MappingExtensions
    {
        public static double ToProgressPercent(this IEnumerable<ProjectTask>? tasks)
        {
            if (tasks is null)
                return 0;

            var list = tasks as ICollection<ProjectTask> ?? tasks.ToList();
            if (list.Count == 0)
                return 0;

            var doneCount = list.Count(t => t.Status == ProjectTaskStatus.Done);
            return Math.Round((double)doneCount / list.Count * 100, 2);
        }

        public static ProjectViewModel ToViewModel(this Project project)
        {
            return new ProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                OwnerId = project.OwnerId,
                OwnerUsername = project.Owner?.Username ?? "",
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                ProgressPercent = project.Tasks.ToProgressPercent()
            };
        }

        public static ProjectDetailsViewModel ToDetailsViewModel(this Project project)
        {
            return new ProjectDetailsViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                OwnerId = project.OwnerId,
                OwnerUsername = project.Owner?.Username ?? "",
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                Members = project.Members?.Select(m => m.ToViewModel()).ToList() ?? new List<MemberViewModel>(),
                Tasks = project.Tasks?.Select(t => t.ToViewModel()).ToList() ?? new List<TaskViewModel>(),
                ProgressPercent = project.Tasks.ToProgressPercent()
            };
        }
    }
}
