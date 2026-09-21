using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Application.Interfaces.Repositories
{
    public interface IProjectRepository
    {
        /*
         
            Functionalities Required: 
            1- Get Project by Id
            2- Get Project Details by Id (readonly)
            3- Get All User Projects (paged, filtered, sorted)
            4- Add Project
            5- Update Project
            6- Delete Project

        */

        Task<Project?> GetProjectByIdAsync(Guid projectId);
        Task<Project?> GetProjectDetailsByIdAsync(Guid projectId);
        Task<PagedResult<Project>> GetUserProjectsAsync(Guid userId, ProjectQueryParameters queryParameters);
        Task<PagedResult<Project>> GetMyOwnedProjectsAsync(Guid userId, ProjectQueryParameters queryParameters);
        Task<PagedResult<Project>> GetMyMemberProjectsAsync(Guid userId, ProjectQueryParameters queryParameters);
        Task AddProjectAsync(Project project);
        void DeleteProject(Project project);
    }
}
