using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using PaymentGateway.Api.Extensions.Authentication;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PaymentGateway.Api.Extensions.Swagger
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerApiDescription(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.DescribeAllParametersInCamelCase();

                AddApiKeyAuthorizeScheme(c);


                // add XML comments
                var commentsFile = Assembly.GetEntryAssembly()?.Location.Replace(".dll", ".xml");
                if (File.Exists(commentsFile))
                {
                    c.IncludeXmlComments(commentsFile, true);
                }
            });

            return services;
        }

        private static void AddApiKeyAuthorizeScheme(SwaggerGenOptions c)
        {
            c.AddSecurityDefinition("api-key", new OpenApiSecurityScheme
            {
                Description = "Default ApiKey Authorization",
                In = ParameterLocation.Header,
                Name = ApiKeyAuthenticationHandler.ApiKeyHeader,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "api-key"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "api-key"
                        },
                        Scheme = "api-key",
                        Name = "api-key",
                        In = ParameterLocation.Header
                    },
                    Array.Empty<string>()
                }
            });
        }

    }
    
}