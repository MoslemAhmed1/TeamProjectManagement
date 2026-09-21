using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Application.Features.Tasks.Queries
{
    public record GetTaskQuery(Guid TaskId, Guid CallerId) : IRequest<TaskViewModel>;

    public class GetTaskQueryHandler(
        IProjectMemberRepository memberRepository,
        IProjectTaskRepository taskRepository)
        : IRequestHandler<GetTaskQuery, TaskViewModel>
    {
        public async Task<TaskViewModel> Handle(GetTaskQuery request, CancellationToken cancellationToken)
        {
            var task = await taskRepository.GetTaskByIdAsync(request.TaskId);
            if (task is null) throw new NotFoundException("Task not found.");

            var isMember = await memberRepository.IsMemberAsync(task.ProjectId, request.CallerId);
            if (!isMember) throw new ForbiddenException("You are not a member of this project.");

            return task.ToViewModel();
        }
    }
}
