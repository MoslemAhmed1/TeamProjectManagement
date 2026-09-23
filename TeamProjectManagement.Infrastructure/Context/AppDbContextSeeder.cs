using TeamProjectManagement.Application.Interfaces.Services;
using TeamProjectManagement.Domain.Entities;
using TeamProjectManagement.Domain.Enums;

namespace TeamProjectManagement.Infrastructure.Context
{
    public static class AppDbContextSeeder
    {
        public static async Task SeedAsync(AppDbContext context, IAuthService authService)
        {
            if (context.Users.Any())
            {
                // Database already seeded
                return;
            }

            var testUser1 = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser1",
                Email = "testuser1@example.com",
                PasswordHash = authService.HashPassword("Test123!"),
                CreatedAt = DateTime.UtcNow
            };

            var testUser2 = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser2",
                Email = "testuser2@example.com",
                PasswordHash = authService.HashPassword("Test123!"),
                CreatedAt = DateTime.UtcNow
            };

            var testUser3 = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser3",
                Email = "testuser3@example.com",
                PasswordHash = authService.HashPassword("Test123!"),
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddRangeAsync(testUser1, testUser2, testUser3);

            var project1 = new Project
            {
                Id = Guid.NewGuid(),
                Name = "Alpha Protocol",
                Description = "A top secret initiative.",
                OwnerId = testUser1.Id,
                CreatedAt = DateTime.UtcNow
            };

            var project2 = new Project
            {
                Id = Guid.NewGuid(),
                Name = "Beta Migration",
                Description = "Migrating legacy systems.",
                OwnerId = testUser2.Id,
                CreatedAt = DateTime.UtcNow
            };

            await context.Projects.AddRangeAsync(project1, project2);

            var member1 = new ProjectMember
            {
                ProjectId = project1.Id,
                UserId = testUser1.Id,
                Role = ProjectMemberRole.Owner
            };

            var member2 = new ProjectMember
            {
                ProjectId = project1.Id,
                UserId = testUser2.Id,
                Role = ProjectMemberRole.Member
            };

            var member3 = new ProjectMember
            {
                ProjectId = project2.Id,
                UserId = testUser2.Id,
                Role = ProjectMemberRole.Owner
            };

            var member4 = new ProjectMember
            {
                ProjectId = project2.Id,
                UserId = testUser3.Id,
                Role = ProjectMemberRole.Member
            };

            var member5 = new ProjectMember
            {
                ProjectId = project1.Id,
                UserId = testUser3.Id,
                Role = ProjectMemberRole.Member
            };

            await context.ProjectMembers.AddRangeAsync(member1, member2, member3, member4, member5);

            var task1 = new ProjectTask
            {
                Id = Guid.NewGuid(),
                ProjectId = project1.Id,
                Title = "Setup Repository",
                Description = "Initialize the git repository with the basic structure.",
                Priority = ProjectTaskPriority.High,
                Status = ProjectTaskStatus.InProgress,
                AssignedToId = testUser2.Id,
                CreatedAt = DateTime.UtcNow
            };

            var task2 = new ProjectTask
            {
                Id = Guid.NewGuid(),
                ProjectId = project1.Id,
                Title = "Configure CI/CD",
                Description = "Set up GitHub Actions for automated testing.",
                Priority = ProjectTaskPriority.Medium,
                Status = ProjectTaskStatus.ToDo,
                AssignedToId = testUser3.Id,
                CreatedAt = DateTime.UtcNow
            };

            var task3 = new ProjectTask
            {
                Id = Guid.NewGuid(),
                ProjectId = project1.Id,
                Title = "Initialize Development Environment",
                Description = "Set up the development environment with necessary tools and dependencies.",
                Priority = ProjectTaskPriority.High,
                Status = ProjectTaskStatus.ToDo,
                AssignedToId = testUser2.Id,
                CreatedAt = DateTime.UtcNow
            };

            var task4 = new ProjectTask
            {
                Id = Guid.NewGuid(),
                ProjectId = project2.Id,
                Title = "Update Documentation",
                Description = "Update the project documentation with the latest changes.",
                Priority = ProjectTaskPriority.Low,
                Status = ProjectTaskStatus.ToDo,
                AssignedToId = testUser3.Id,
                CreatedAt = DateTime.UtcNow
            };

            await context.ProjectTasks.AddRangeAsync(task1, task2, task3, task4);

            await context.SaveChangesAsync();
        }
    }
}
