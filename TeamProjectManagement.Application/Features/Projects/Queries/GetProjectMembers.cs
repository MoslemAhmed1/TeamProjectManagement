using MediatR;
using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Application.Features.Projects.Queries
{
    public record GetProjectMembersQuery(Guid ProjectId, Guid CallerId, QueryParameters Parameters) : IRequest<PagedViewModel<MemberViewModel>>;

    public class GetProjectMembersQueryHandler(
        IProjectRepository projectRepository,
        IProjectMemberRepository memberRepository)
        : IRequestHandler<GetProjectMembersQuery, PagedViewModel<MemberViewModel>>
    {
        public async Task<PagedViewModel<MemberViewModel>> Handle(GetProjectMembersQuery request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.GetProjectByIdAsync(request.ProjectId);
            if (project is null) throw new NotFoundException("Project not found.");

            var isMember = await memberRepository.IsMemberAsync(request.ProjectId, request.CallerId);
            if (!isMember) throw new ForbiddenException("You are not a member of this project.");

            var paged = await memberRepository.GetMembersAsync(request.ProjectId, request.Parameters, request.CallerId);

            return paged.ToPagedViewModel(m => m.ToViewModel());
        }
    }
}
