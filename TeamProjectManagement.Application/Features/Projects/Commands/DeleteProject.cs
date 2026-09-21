using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;

namespace TeamProjectManagement.Application.Features.Projects.Commands
{
    public record DeleteProjectCommand(Guid ProjectId, Guid CallerId) : IRequest;

    public class DeleteProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteProjectCommand>
    {
        public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.GetProjectByIdAsync(request.ProjectId);
            if (project is null) throw new NotFoundException("Project not found.");

            if (project.OwnerId != request.CallerId)
                throw new ForbiddenException("Only the project owner can delete the project.");

            projectRepository.DeleteProject(project);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
