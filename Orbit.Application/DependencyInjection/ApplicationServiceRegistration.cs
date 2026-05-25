using Microsoft.Extensions.DependencyInjection;
using Orbit.Application.Services.Parsing;

namespace Orbit.Application.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<CurriculumStructureParser>();
            
            return services;
        }
    }
}
