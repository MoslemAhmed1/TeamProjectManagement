using Microsoft.EntityFrameworkCore;
using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Domain.Entities;
using TeamProjectManagement.Infrastructure.Context;
using TeamProjectManagement.Infrastructure.Repositories.Extensions;

namespace TeamProjectManagement.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Project?> GetProjectByIdAsync(Guid projectId)
        {
            return await _context.Projects.FindAsync(projectId);
        }

        public async Task<Project?> GetProjectDetailsByIdAsync(Guid projectId)
        {
            return await _context.Projects
                .AsNoTracking()
                .Include(p => p.Owner)
                .Include(p => p.Members).ThenInclude(m => m.User)
                .Include(p => p.Tasks).ThenInclude(t => t.AssignedTo)
                .FirstOrDefaultAsync(p => p.Id == projectId);
        }

        public async Task<PagedResult<Project>> GetUserProjectsAsync(Guid userId, ProjectQueryParameters queryParameters)
        {
            var query = _context.Projects
                .AsNoTracking()
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId));

            // Search by name or description
            if (!string.IsNullOrWhiteSpace(queryParameters.SearchTerm))
            {
                var searchTerm = queryParameters.SearchTerm.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm))
                );
            }

            // Sort
            query = query.ApplySort(queryParameters.SortBy, queryParameters.SortDescending);

            return await query.ToPagedResultAsync(queryParameters.PageNumber, queryParameters.PageSize);
        }

        public async Task AddProjectAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
        }

        public void DeleteProject(Project project)
        {
            _context.Projects.Remove(project);
        }
    }
}
