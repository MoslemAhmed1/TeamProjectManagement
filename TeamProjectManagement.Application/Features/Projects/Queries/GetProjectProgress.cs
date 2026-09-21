using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Application.Features.Projects.Queries
{
    public record GetProjectProgressQuery(Guid ProjectId, Guid CallerId) : IRequest<ProgressViewModel>;

    public class GetProjectProgressQueryHandler(
        IProjectRepository projectRepository,
        IProjectMemberRepository memberRepository,
        IProjectTaskRepository taskRepository)
        : IRequestHandler<GetProjectProgressQuery, ProgressViewModel>
    {
        public async Task<ProgressViewModel> Handle(GetProjectProgressQuery request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.GetProjectByIdAsync(request.ProjectId);
            if (project is null) throw new NotFoundException("Project not found.");

            var isMember = await memberRepository.IsMemberAsync(request.ProjectId, request.CallerId);
            if (!isMember) throw new ForbiddenException("You are not a member of this project.");

            var progress = await taskRepository.GetProjectProgressAsync(request.ProjectId);

            return new ProgressViewModel
            {
                ProjectId = request.ProjectId,
                ProgressPercent = progress
            };
        }
    }
}
