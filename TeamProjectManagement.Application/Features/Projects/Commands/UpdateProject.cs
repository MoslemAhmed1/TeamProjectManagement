using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;

namespace TeamProjectManagement.Application.Features.Projects.Commands
{
    public record UpdateProjectCommand(Guid ProjectId, string Name, string? Description, Guid CallerId) : IRequest;

    public class UpdateProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateProjectCommand>
    {
        public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.GetProjectByIdAsync(request.ProjectId);
            if (project is null) throw new NotFoundException("Project not found.");

            if (project.OwnerId != request.CallerId)
                throw new ForbiddenException("Only the project owner can update the project.");

            project.Name = request.Name.Trim();
            project.Description = request.Description?.Trim();

            await unitOfWork.SaveChangesAsync();
        }
    }
}
