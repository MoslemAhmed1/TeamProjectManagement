using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;
using TeamProjectManagement.Domain.Entities;
using TeamProjectManagement.Domain.Enums;

namespace TeamProjectManagement.Application.Features.Projects.Commands
{
    public record CreateProjectCommand(
        [Required] [StringLength(150, MinimumLength = 1)] string Name,
        [StringLength(1000)] string? Description,
        Guid CallerId) : IRequest<ProjectViewModel>;

    public class CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IProjectMemberRepository memberRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateProjectCommandHandler> logger)
        : IRequestHandler<CreateProjectCommand, ProjectViewModel>
    {
        public async Task<ProjectViewModel> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
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

            logger.LogInformation("User {UserId} created project {ProjectId}", request.CallerId, project.Id);
            return project.ToViewModel();
        }
    }
}
