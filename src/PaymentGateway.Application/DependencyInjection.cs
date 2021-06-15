using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application.Cards;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Application.Common.Behaviours;

namespace PaymentGateway.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(DependencyInjection).Assembly);
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            return services;
        }
    }
}