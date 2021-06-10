using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application.Abstractions;

namespace TemplateApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            return services;
        }
    }
}