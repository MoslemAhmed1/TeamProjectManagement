using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TeamProjectManagement.Application.Settings;
using TeamProjectManagement.Infrastructure.Context;
using TeamProjectManagement.Api.Middlewares;
using TeamProjectManagement.Application;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_URL"));
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? string.Empty;

builder.Services.Configure<JwtSettings>(options =>
{
    builder.Configuration.GetSection("Jwt").Bind(options);
    options.Secret = jwtSecret;
});

builder.Services.AddAppDependencies();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

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

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
