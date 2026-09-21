using TeamProjectManagement.Application.ViewModels;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Application.Mappings
{
    public static partial class MappingExtensions
    {
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
                UpdatedAt = project.UpdatedAt
            };
        }

        public static ProjectDetailsViewModel ToDetailsViewModel(this Project project, double progressPercent)
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
                ProgressPercent = progressPercent
            };
        }
    }
}
