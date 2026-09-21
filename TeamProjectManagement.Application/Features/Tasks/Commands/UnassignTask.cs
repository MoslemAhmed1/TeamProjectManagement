using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;

namespace TeamProjectManagement.Application.Features.Tasks.Commands
{
    public record UnassignTaskCommand(Guid TaskId, Guid CallerId) : IRequest;

    public class UnassignTaskCommandHandler(
        IProjectRepository projectRepository,
        IProjectTaskRepository taskRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<UnassignTaskCommand>
    {
        public async Task Handle(UnassignTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await taskRepository.GetTaskByIdAsync(request.TaskId);
            if (task is null) throw new NotFoundException("Task not found.");

            var project = await projectRepository.GetProjectByIdAsync(task.ProjectId);
            if (project is null || project.OwnerId != request.CallerId)
                throw new ForbiddenException("Only the project owner can unassign tasks.");

            task.AssignedToId = null;
            await unitOfWork.SaveChangesAsync();
        }
    }
}
