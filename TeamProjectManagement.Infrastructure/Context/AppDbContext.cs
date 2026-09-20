using Microsoft.EntityFrameworkCore;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is User user)
                        user.CreatedAt = now;

                    if (entry.Entity is Project project)
                    {
                        project.CreatedAt = now;
                        project.UpdatedAt = now;
                    }

                    if (entry.Entity is ProjectTask task)
                    {
                        task.CreatedAt = now;
                        task.UpdatedAt = now;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is Project project)
                        project.UpdatedAt = now;

                    if (entry.Entity is ProjectTask task)
                        task.UpdatedAt = now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
