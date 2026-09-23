using MediatR;
using System.ComponentModel.DataAnnotations;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Domain.Enums;

namespace TeamProjectManagement.Application.Features.Tasks.Commands
{
    public record UpdateTaskCommand(
        Guid TaskId,
        [Required] [StringLength(200, MinimumLength = 1)] string Title,
        [StringLength(2000)] string? Description,
        ProjectTaskPriority Priority,
        ProjectTaskStatus Status,
        DateTime? DueAt,
        Guid CallerId) : IRequest;

    public class UpdateTaskCommandHandler(
        IProjectRepository projectRepository,
        IProjectTaskRepository taskRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateTaskCommand>
    {
        public async Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await taskRepository.GetTaskByIdAsync(request.TaskId);
            if (task is null) throw new NotFoundException("Task not found.");

            var project = await projectRepository.GetProjectByIdAsync(task.ProjectId);
            if (project is null || project.OwnerId != request.CallerId)
                throw new ForbiddenException("Only the project owner can fully update tasks.");

            task.Title = request.Title;
            task.Description = request.Description;
            task.Priority = request.Priority;
            task.Status = request.Status;
            task.DueAt = request.DueAt?.ToUniversalTime();

            await unitOfWork.SaveChangesAsync();
        }
    }
}
