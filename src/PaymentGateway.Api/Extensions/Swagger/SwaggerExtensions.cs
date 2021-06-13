using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
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
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"Enter Bearer [space] and then your token in the text input below. 
                        Example: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });
        }

    }
    
}