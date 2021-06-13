using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api.Extensions.Authentication;
using PaymentGateway.Infrastructure;
using PaymentGateway.Infrastructure.Persistence;

namespace PaymentGateway.Api.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services => ConfigureServices(services))
                .ConfigureTestServices(_ => { });
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
            var result = appDb.Database.EnsureCreated();
            return services;
        }


    }

    public static class ServiceCollectionExtensions
    {
        public static HttpClient CreateClientWithShopperId(this WebApplicationFactory<Startup> factory, Guid shopperId = default)
        {
            var testClient = factory.CreateClient();
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();
            var tokenGenerator = new JsonWebTokenGenerator(new DateTimeProvider(), builder);
            var token = tokenGenerator.GenerateJSONWebToken(shopperId);
            testClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
            return testClient;
        }
        public static IServiceCollection AddDbContext(this IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault
                (d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
            return services;
        }
    }
}