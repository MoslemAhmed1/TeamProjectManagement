using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;

namespace TeamProjectManagement.Application.Features.Projects.Commands
{
    public record RemoveProjectMemberCommand(Guid ProjectId, Guid TargetUserId, Guid CallerId) : IRequest;

    public class RemoveProjectMemberCommandHandler(
        IProjectRepository projectRepository,
        IProjectMemberRepository memberRepository,
        IProjectTaskRepository taskRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<RemoveProjectMemberCommand>
    {
        public async Task Handle(RemoveProjectMemberCommand request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.GetProjectByIdAsync(request.ProjectId);
            if (project is null) throw new NotFoundException("Project not found.");

            if (project.OwnerId != request.CallerId)
                throw new ForbiddenException("Only the project owner can remove members.");

            if (request.TargetUserId == project.OwnerId)
                throw new BadRequestException("The project owner cannot be removed.");

            var membership = await memberRepository.GetMemberAsync(request.ProjectId, request.TargetUserId);
            if (membership is null) throw new NotFoundException("User is not a member of this project.");

            await unitOfWork.BeginTransactionAsync();
            try
            {
                await taskRepository.UnassignAllTasksAsync(request.ProjectId, request.TargetUserId);
                memberRepository.RemoveMember(membership);
                await unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
