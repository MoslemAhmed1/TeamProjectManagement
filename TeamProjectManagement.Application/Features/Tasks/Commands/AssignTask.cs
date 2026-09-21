using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;

namespace TeamProjectManagement.Application.Features.Tasks.Commands
{
    public record AssignTaskCommand(Guid TaskId, Guid AssignedToId, Guid CallerId) : IRequest;

    public class AssignTaskCommandHandler(
        IProjectRepository projectRepository,
        IProjectMemberRepository memberRepository,
        IProjectTaskRepository taskRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<AssignTaskCommand>
    {
        public async Task Handle(AssignTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await taskRepository.GetTaskByIdAsync(request.TaskId);
            if (task is null) throw new NotFoundException("Task not found.");

            var project = await projectRepository.GetProjectByIdAsync(task.ProjectId);
            if (project is null || project.OwnerId != request.CallerId)
                throw new ForbiddenException("Only the project owner can assign tasks.");

            var assignee = await userRepository.GetUserByIdAsync(request.AssignedToId);
            if (assignee is null) throw new NotFoundException("User to assign not found.");

            var isMember = await memberRepository.IsMemberAsync(task.ProjectId, request.AssignedToId);
            if (!isMember) throw new BadRequestException("User must be a member of the project to be assigned a task.");

            task.AssignedToId = request.AssignedToId;
            await unitOfWork.SaveChangesAsync();
        }
    }
}
