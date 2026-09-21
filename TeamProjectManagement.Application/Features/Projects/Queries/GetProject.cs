using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Application.Features.Projects.Queries
{
    public record GetProjectQuery(Guid ProjectId, Guid CallerId) : IRequest<ProjectDetailsViewModel>;

    public class GetProjectQueryHandler(
        IProjectRepository projectRepository,
        IProjectMemberRepository memberRepository,
        IProjectTaskRepository taskRepository)
        : IRequestHandler<GetProjectQuery, ProjectDetailsViewModel>
    {
        public async Task<ProjectDetailsViewModel> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.GetProjectDetailsByIdAsync(request.ProjectId);
            if (project is null) throw new NotFoundException("Project not found.");

            var isMember = await memberRepository.IsMemberAsync(request.ProjectId, request.CallerId);
            if (!isMember) throw new ForbiddenException("You are not a member of this project.");

            var progress = await taskRepository.GetProjectProgressAsync(request.ProjectId);

            return project.ToDetailsViewModel(progress);
        }
    }
}
