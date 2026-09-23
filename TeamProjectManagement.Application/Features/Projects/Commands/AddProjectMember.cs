using MediatR;
using Microsoft.Extensions.Logging;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;
using TeamProjectManagement.Domain.Entities;
using TeamProjectManagement.Domain.Enums;

namespace TeamProjectManagement.Application.Features.Projects.Commands
{
    public record AddProjectMemberCommand(Guid ProjectId, Guid UserId, Guid CallerId) : IRequest<MemberViewModel>;

    public class AddProjectMemberCommandHandler(
        IProjectRepository projectRepository,
        IProjectMemberRepository memberRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddProjectMemberCommandHandler> logger)
        : IRequestHandler<AddProjectMemberCommand, MemberViewModel>
    {
        public async Task<MemberViewModel> Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.GetProjectByIdAsync(request.ProjectId);
            if (project is null) throw new NotFoundException("Project not found.");

            if (project.OwnerId != request.CallerId)
                throw new ForbiddenException("Only the project owner can add members.");

            var targetUser = await userRepository.GetUserByIdAsync(request.UserId);
            if (targetUser is null) throw new NotFoundException("User not found.");

            if (request.UserId == request.CallerId)
                throw new BadRequestException("The project owner is already a member.");

            var isMember = await memberRepository.IsMemberAsync(request.ProjectId, request.UserId);
            if (isMember) throw new ConflictException("User is already a member of this project.");

            var member = new ProjectMember
            {
                ProjectId = request.ProjectId,
                UserId = request.UserId,
                Role = ProjectMemberRole.Member,
                User = targetUser
            };

            await memberRepository.AddMemberAsync(member);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("User {UserId} added member {MemberId} to project {ProjectId}", request.CallerId, request.UserId, request.ProjectId);
            return member.ToViewModel();
        }
    }
}
