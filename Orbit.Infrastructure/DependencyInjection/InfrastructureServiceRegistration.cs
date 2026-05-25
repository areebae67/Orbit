using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orbit.Infrastructure.AI;
using Orbit.Infrastructure.Pdf;
using Orbit.Infrastructure.Services;

namespace Orbit.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<CurriculumTextProcessor>();
            services.AddScoped<StructureInferenceEngine>();
            services.AddScoped<CourseBlockBuilder>();
            services.AddScoped<CurriculumIntelligenceEngine>();
            services.AddScoped<CurriculumComparisonEngine>();

            services.AddHttpClient<AiEnrichmentService>(client =>
            {
                client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
                client.Timeout = TimeSpan.FromSeconds(60);
            });

            services.AddHttpClient<GeminiCurriculumParsingService>(client =>
            {
                client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
                client.Timeout = TimeSpan.FromSeconds(120);
            });

            return services;
        }
    }
}
