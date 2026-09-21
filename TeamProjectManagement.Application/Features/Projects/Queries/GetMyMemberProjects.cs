using MediatR;
using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Application.Features.Projects.Queries
{
    public record GetMyMemberProjectsQuery(Guid CallerId, ProjectQueryParameters Parameters) : IRequest<PagedViewModel<ProjectViewModel>>;

    public class GetMyMemberProjectsQueryHandler(
        IProjectRepository projectRepository)
        : IRequestHandler<GetMyMemberProjectsQuery, PagedViewModel<ProjectViewModel>>
    {
        public async Task<PagedViewModel<ProjectViewModel>> Handle(GetMyMemberProjectsQuery request, CancellationToken cancellationToken)
        {
            var paged = await projectRepository.GetMyMemberProjectsAsync(request.CallerId, request.Parameters);
            return paged.ToPagedViewModel(p => p.ToViewModel());
        }
    }
}
