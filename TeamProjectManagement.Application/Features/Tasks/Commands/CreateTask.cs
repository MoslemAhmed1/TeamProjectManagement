using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;
using TeamProjectManagement.Domain.Entities;
using TeamProjectManagement.Domain.Enums;

namespace TeamProjectManagement.Application.Features.Tasks.Commands
{
    public record CreateTaskCommand(
        Guid ProjectId,
        string Title,
        string? Description,
        ProjectTaskPriority Priority,
        DateTime? DueAt,
        Guid? AssignedToId,
        Guid CallerId) : IRequest<TaskViewModel>;

    public class CreateTaskCommandHandler(
        IProjectRepository projectRepository,
        IProjectMemberRepository memberRepository,
        IProjectTaskRepository taskRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<CreateTaskCommand, TaskViewModel>
    {
        public async Task<TaskViewModel> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.GetProjectByIdAsync(request.ProjectId);
            if (project is null) throw new NotFoundException("Project not found.");

            if (project.OwnerId != request.CallerId)
                throw new ForbiddenException("Only the project owner can create tasks.");

            if (request.AssignedToId.HasValue)
            {
                var assignee = await userRepository.GetUserByIdAsync(request.AssignedToId.Value);
                if (assignee is null) throw new NotFoundException("Assigned user not found.");

                var assigneeIsMember = await memberRepository.IsMemberAsync(request.ProjectId, request.AssignedToId.Value);
                if (!assigneeIsMember) throw new BadRequestException("Assigned user is not a member of this project.");
            }

            var task = new ProjectTask
            {
                Id = Guid.NewGuid(),
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                Priority = request.Priority,
                Status = ProjectTaskStatus.ToDo,
                DueAt = request.DueAt?.ToUniversalTime(),
                ProjectId = request.ProjectId,
                AssignedToId = request.AssignedToId
            };

            await taskRepository.AddProjectTaskAsync(task);
            await unitOfWork.SaveChangesAsync();

            return task.ToViewModel();
        }
    }
}
