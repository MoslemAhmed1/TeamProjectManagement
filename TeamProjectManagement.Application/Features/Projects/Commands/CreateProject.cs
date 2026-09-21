using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;
using TeamProjectManagement.Domain.Entities;
using TeamProjectManagement.Domain.Enums;

namespace TeamProjectManagement.Application.Features.Projects.Commands
{
    public record CreateProjectCommand(string Name, string? Description, Guid CallerId) : IRequest<ProjectViewModel>;

    public class CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IProjectMemberRepository memberRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<CreateProjectCommand, ProjectViewModel>
    {
        public async Task<ProjectViewModel> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                OwnerId = request.CallerId,
            };

            var member = new ProjectMember
            {
                ProjectId = project.Id,
                UserId = request.CallerId,
                Role = ProjectMemberRole.Owner,
            };

            await unitOfWork.BeginTransactionAsync();
            try
            {
                await projectRepository.AddProjectAsync(project);
                await memberRepository.AddMemberAsync(member);
                await unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }

            return project.ToViewModel();
        }
    }
}
