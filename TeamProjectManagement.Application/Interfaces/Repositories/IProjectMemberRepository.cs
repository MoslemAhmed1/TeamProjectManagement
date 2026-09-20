using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Domain.Entities;
using TeamProjectManagement.Domain.Enums;

namespace TeamProjectManagement.Application.Interfaces.Repositories
{
    public interface IProjectMemberRepository
    {
        /*

            Functionalities Required: 
            1- Get Role by (ProjectId, UserId)
            2- Is Member by (ProjectId, UserId)
            3- Get Members by ProjectId (paged)
            4- Add Member (ProjectId, UserId, Role)
            5- Remove Member (ProjectId, UserId)

        */

        Task<ProjectMemberRole?> GetRoleAsync(Guid projectId, Guid userId);
        Task<bool> IsMemberAsync(Guid projectId, Guid userId);
        Task<PagedResult<ProjectMember>> GetMembersAsync(Guid projectId, QueryParameters queryParameters);
        Task AddMemberAsync(ProjectMember member);
        void RemoveMember(ProjectMember member);
    }
}
