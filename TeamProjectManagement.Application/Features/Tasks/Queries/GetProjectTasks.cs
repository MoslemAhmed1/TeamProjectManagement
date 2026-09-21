using MediatR;
using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Application.Features.Tasks.Queries
{
    public record GetProjectTasksQuery(Guid ProjectId, Guid CallerId, TaskQueryParameters Parameters) : IRequest<PagedViewModel<TaskViewModel>>;

    public class GetProjectTasksQueryHandler(
        IProjectRepository projectRepository,
        IProjectMemberRepository memberRepository,
        IProjectTaskRepository taskRepository)
        : IRequestHandler<GetProjectTasksQuery, PagedViewModel<TaskViewModel>>
    {
        public async Task<PagedViewModel<TaskViewModel>> Handle(GetProjectTasksQuery request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.GetProjectByIdAsync(request.ProjectId);
            if (project is null) throw new NotFoundException("Project not found.");

            var isMember = await memberRepository.IsMemberAsync(request.ProjectId, request.CallerId);
            if (!isMember) throw new ForbiddenException("You are not a member of this project.");

            var paged = await taskRepository.GetTasksByProjectIdAsync(request.ProjectId, request.Parameters);
            return paged.ToPagedViewModel(t => t.ToViewModel());
        }
    }
}
