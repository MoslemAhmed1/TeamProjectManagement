using Microsoft.EntityFrameworkCore;
using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Domain.Entities;
using TeamProjectManagement.Domain.Enums;
using TeamProjectManagement.Infrastructure.Context;
using TeamProjectManagement.Infrastructure.Repositories.Extensions;

namespace TeamProjectManagement.Infrastructure.Repositories
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly AppDbContext _context;

        public ProjectMemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectMemberRole?> GetRoleAsync(Guid projectId, Guid userId)
        {
            var member = await _context.ProjectMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

            return member?.Role;
        }

        public async Task<ProjectMember?> GetMemberAsync(Guid projectId, Guid userId)
        {
            return await _context.ProjectMembers
                .Include(pm => pm.User)
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
        }

        public async Task<bool> IsMemberAsync(Guid projectId, Guid userId)
        {
            return await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
        }

        public async Task<PagedResult<ProjectMember>> GetMembersAsync(Guid projectId, QueryParameters queryParameters, Guid? excludeUserId = null)
        {
            var query = _context.ProjectMembers
                .AsNoTracking()
                .Include(pm => pm.User)
                .Where(pm => pm.ProjectId == projectId);

            if (excludeUserId.HasValue)
            {
                query = query.Where(pm => pm.UserId != excludeUserId.Value);
            }

            return await query.ToPagedResultAsync(queryParameters.PageNumber, queryParameters.PageSize);
        }

        public async Task AddMemberAsync(ProjectMember member)
        {
            await _context.ProjectMembers.AddAsync(member);
        }

        public void RemoveMember(ProjectMember member)
        {
            _context.ProjectMembers.Remove(member);
        }
    }
}
