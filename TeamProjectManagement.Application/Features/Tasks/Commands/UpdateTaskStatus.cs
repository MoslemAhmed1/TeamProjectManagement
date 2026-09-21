using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Domain.Enums;

namespace TeamProjectManagement.Application.Features.Tasks.Commands
{
    public record UpdateTaskStatusCommand(Guid TaskId, ProjectTaskStatus Status, Guid CallerId) : IRequest;

    public class UpdateTaskStatusCommandHandler(
        IProjectRepository projectRepository,
        IProjectMemberRepository memberRepository,
        IProjectTaskRepository taskRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateTaskStatusCommand>
    {
        public async Task Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var task = await taskRepository.GetTaskByIdAsync(request.TaskId);
            if (task is null) throw new NotFoundException("Task not found.");

            var project = await projectRepository.GetProjectByIdAsync(task.ProjectId);
            if (project is null) throw new NotFoundException("Project not found.");

            var isMember = await memberRepository.IsMemberAsync(task.ProjectId, request.CallerId);
            if (!isMember) throw new ForbiddenException("You are not a member of this project.");

            bool isOwner = project.OwnerId == request.CallerId;
            bool isAssignee = task.AssignedToId == request.CallerId;
            if (!isOwner && !isAssignee)
                throw new ForbiddenException("Members can only update the status of tasks assigned to them.");

            task.Status = request.Status;
            await unitOfWork.SaveChangesAsync();
        }
    }
}
