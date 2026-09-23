using Microsoft.Extensions.DependencyInjection;
using TeamProjectManagement.Application.Features.Users.Commands;

namespace TeamProjectManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<LoginUserCommandHandler>());
            return services;
        }
    }
}
