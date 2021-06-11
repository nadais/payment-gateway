using System;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api.Extensions.Authentication;
using PaymentGateway.Infrastructure.Persistence;

namespace PaymentGateway.Api.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public static string ApiKey => "FROM_ENV";
        private readonly Action<IServiceCollection> _action;
        public CustomWebApplicationFactory(Action<IServiceCollection> action)
        {
            _action = action;
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(null)
                  .ConfigureServices(services => ConfigureServices(services))
                .ConfigureTestServices(services => { })
                .UseStartup<Startup>();
        }

        public HttpClient CreateClientWithDefaultApiKey(Action<HttpClient> options = null)
        {
            var testClient = CreateClient();
            testClient.DefaultRequestHeaders.TryAddWithoutValidation(ApiKeyAuthenticationHandler.ApiKeyHeader, ApiKey);
            options?.Invoke(testClient);
            return testClient;
        }

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext();
            services.AddMvc(options =>
            {
                options.Filters.Add(new AllowAnonymousFilter());
            }).AddApplicationPart(Assembly.Load(typeof(Startup).Assembly.GetName()));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var appDb = scopedServices.GetRequiredService<AppDbContext>();

            // Ensure the database is created.
            appDb.Database.EnsureCreated();
            _action?.Invoke(services);
            return services;
        }


    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlite()
                .BuildServiceProvider();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite("DataSource=:memory:");
                options.UseInternalServiceProvider(serviceProvider);
            });
            return services;
        }
    }
}