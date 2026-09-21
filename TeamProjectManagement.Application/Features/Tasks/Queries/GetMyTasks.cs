using MediatR;
using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Application.Features.Tasks.Queries
{
    public record GetMyTasksQuery(Guid CallerId, TaskQueryParameters Parameters) : IRequest<PagedViewModel<TaskViewModel>>;

    public class GetMyTasksQueryHandler(IProjectTaskRepository taskRepository)
        : IRequestHandler<GetMyTasksQuery, PagedViewModel<TaskViewModel>>
    {
        public async Task<PagedViewModel<TaskViewModel>> Handle(GetMyTasksQuery request, CancellationToken cancellationToken)
        {
            var paged = await taskRepository.GetTasksByUserIdAsync(request.CallerId, request.Parameters);
            return paged.ToPagedViewModel(t => t.ToViewModel());
        }
    }
}
