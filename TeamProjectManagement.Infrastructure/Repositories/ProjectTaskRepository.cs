using Microsoft.EntityFrameworkCore;
using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Domain.Entities;
using TeamProjectManagement.Domain.Enums;
using TeamProjectManagement.Infrastructure.Context;
using TeamProjectManagement.Infrastructure.Repositories.Extensions;

namespace TeamProjectManagement.Infrastructure.Repositories
{
    public class ProjectTaskRepository : IProjectTaskRepository
    {
        private readonly AppDbContext _context;

        public ProjectTaskRepository(AppDbContext context)
        {
            _context = context;
        }



        public async Task<ProjectTask?> GetTaskByIdAsync(Guid taskId)
        {
            return await _context.ProjectTasks
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async Task<PagedResult<ProjectTask>> GetTasksByProjectIdAsync(Guid projectId, TaskQueryParameters queryParameters)
        {
            var query = _context.ProjectTasks
                .AsNoTracking()
                .Include(t => t.AssignedTo)
                .Where(t => t.ProjectId == projectId);

            query = ApplyTaskFilters(query, queryParameters);

            return await query.ToPagedResultAsync(queryParameters.PageNumber, queryParameters.PageSize);
        }

        public async Task<PagedResult<ProjectTask>> GetTasksByUserIdAsync(Guid userId, TaskQueryParameters queryParameters)
        {
            var query = _context.ProjectTasks
                .AsNoTracking()
                .Include(t => t.AssignedTo)
                .Include(t => t.Project)
                .Where(t => t.AssignedToId == userId);

            query = ApplyTaskFilters(query, queryParameters);
            query = query.ApplySort(queryParameters.SortBy, queryParameters.SortDescending);

            return await query.ToPagedResultAsync(queryParameters.PageNumber, queryParameters.PageSize);
        }

        public async Task AddProjectTaskAsync(ProjectTask projectTask)
        {
            await _context.ProjectTasks.AddAsync(projectTask);
        }



        public void DeleteProjectTask(ProjectTask projectTask)
        {
            _context.ProjectTasks.Remove(projectTask);
        }

        public async Task<double> GetProjectProgressAsync(Guid projectId)
        {
            var tasks = await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId)
                .Select(t => t.Status)
                .ToListAsync();

            if (tasks.Count == 0)
                return 0.0;

            var doneCount = tasks.Count(s => s == ProjectTaskStatus.Done);
            return Math.Round((double)doneCount / tasks.Count * 100, 2);
        }

        public async Task UnassignAllTasksAsync(Guid projectId, Guid userId)
        {
            await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId && t.AssignedToId == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.AssignedToId, (Guid?)null));
        }

        private static IQueryable<ProjectTask> ApplyTaskFilters(IQueryable<ProjectTask> query, TaskQueryParameters parameters)
        {
            // Filter by status
            if (parameters.Status.HasValue)
                query = query.Where(t => t.Status == parameters.Status.Value);

            // Filter by priority
            if (parameters.Priority.HasValue)
                query = query.Where(t => t.Priority == parameters.Priority.Value);

            // Filter by assignee
            if (parameters.AssignedToId.HasValue)
                query = query.Where(t => t.AssignedToId == parameters.AssignedToId.Value);

            // Filter by due date range
            if (parameters.DueBefore.HasValue)
                query = query.Where(t => t.DueAt != null && t.DueAt <= parameters.DueBefore.Value);

            if (parameters.DueAfter.HasValue)
                query = query.Where(t => t.DueAt != null && t.DueAt >= parameters.DueAfter.Value);

            // Search by title or description
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(searchTerm) ||
                    (t.Description != null && t.Description.ToLower().Contains(searchTerm)));
            }

            return query;
        }
    }
}
