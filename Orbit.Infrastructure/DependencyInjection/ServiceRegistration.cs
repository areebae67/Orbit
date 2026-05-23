using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Orbit.Infrastructure.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            return services;
        }
    }
}