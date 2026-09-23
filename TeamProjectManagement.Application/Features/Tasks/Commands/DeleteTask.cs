using MediatR;
using Microsoft.Extensions.Logging;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;

namespace TeamProjectManagement.Application.Features.Tasks.Commands
{
    public record DeleteTaskCommand(Guid TaskId, Guid CallerId) : IRequest;

    public class DeleteTaskCommandHandler(
        IProjectRepository projectRepository,
        IProjectTaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteTaskCommandHandler> logger)
        : IRequestHandler<DeleteTaskCommand>
    {
        public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await taskRepository.GetTaskByIdAsync(request.TaskId);
            if (task is null) throw new NotFoundException("Task not found.");

            var project = await projectRepository.GetProjectByIdAsync(task.ProjectId);
            if (project is null || project.OwnerId != request.CallerId)
                throw new ForbiddenException("Only the project owner can delete tasks.");

            taskRepository.DeleteProjectTask(task);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("User {UserId} deleted task {TaskId}", request.CallerId, task.Id);
        }
    }
}
