using MediatR;
using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Application.Features.Projects.Queries
{
    public record GetMyOwnedProjectsQuery(Guid CallerId, ProjectQueryParameters Parameters) : IRequest<PagedViewModel<ProjectViewModel>>;

    public class GetMyOwnedProjectsQueryHandler(
        IProjectRepository projectRepository)
        : IRequestHandler<GetMyOwnedProjectsQuery, PagedViewModel<ProjectViewModel>>
    {
        public async Task<PagedViewModel<ProjectViewModel>> Handle(GetMyOwnedProjectsQuery request, CancellationToken cancellationToken)
        {
            var paged = await projectRepository.GetMyOwnedProjectsAsync(request.CallerId, request.Parameters);
            return paged.ToPagedViewModel(p => p.ToViewModel());
        }
    }
}
