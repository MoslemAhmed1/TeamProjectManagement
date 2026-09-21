using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Application.Interfaces.Repositories
{
    public interface IProjectTaskRepository
    {
        /*

            Functionalities Required: 
            1- Get By (ProjectId, TaskId)
            2- Get All By ProjectId (paged, filtered, sorted)
            3- Get All By UserId (paged, filtered, sorted)
            4- Create Task
            5- Update Task
            6- Delete Task
            7- Get Project Progress By ProjectId
            8- Unassign All Tasks By (ProjectId, UserId)

        */


        Task<ProjectTask?> GetTaskByIdAsync(Guid taskId);
        Task<PagedResult<ProjectTask>> GetTasksByProjectIdAsync(Guid projectId, TaskQueryParameters queryParameters);
        Task<PagedResult<ProjectTask>> GetTasksByUserIdAsync(Guid userId, TaskQueryParameters queryParameters);
        Task AddProjectTaskAsync(ProjectTask projectTask);

        void DeleteProjectTask(ProjectTask projectTask);
        Task<double> GetProjectProgressAsync(Guid projectId);
        Task UnassignAllTasksAsync(Guid projectId, Guid userId);
    }
}
