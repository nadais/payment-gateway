using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Domain.Bank;
using PaymentGateway.Infrastructure.Clients;
using PaymentGateway.Infrastructure.Persistence;
using PaymentGateway.Infrastructure.Services;
using Refit;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace PaymentGateway.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IBankService, BankService>();
            AddDbContext(services, configuration);
            services.AddBankService();
            return services;
        }
        private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            services.AddScoped<IAppDbContext, AppDbContext>();
        }
        
        private static void AddBankService(this IServiceCollection services)
        {
            var server = WireMockServer.Start();
            server.Given(Request.Create().WithPath("/transfer").UsingPost()
                .WithBody(new JsonPathMatcher("$.quantity", "$.currency")))
                .RespondWith(
                    Response.Create().WithBodyAsJson(new BankPaymentResponse
                    {
                        Id = Guid.NewGuid(),
                        IsSuccessful = true
                    }));
            server.Given(Request.Create().WithPath("/transfer").UsingPost())
                .RespondWith(
                    Response.Create().WithBodyAsJson(new BankPaymentResponse
                    {
                        Id = Guid.NewGuid(),
                        IsSuccessful = true
                    }));
            
            services
                .AddRefitClient<IBankServiceClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(server.Urls[0]));
        }
    }
}