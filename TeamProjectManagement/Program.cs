using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TeamProjectManagement.Infrastructure.Context;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_URL"));
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Automatic Migartions
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        if (dbContext.Database.HasPendingModelChanges())
        {
            logger.LogCritical("There are changes in Domain.Entities which haven't been added to a migration.");
            throw new InvalidOperationException("Add a migration before starting the application.");
        }

        logger.LogInformation("Applying database migrations...");
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied.");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Database migration failed.");
        throw;
    }
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Add Swagger
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
