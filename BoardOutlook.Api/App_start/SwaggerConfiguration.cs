using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BoardOutlook.Api.App_start
{
    // <summary>
    /// Swagger bootstrap configuration
    /// </summary>
    public static class SwaggerConfiguration
    {
        /// <summary>
        /// Enable swagger for the project
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IApplicationBuilder EnableSwagger(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseSwagger()
            .UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Teach Portal v1"));

            return app;
        }

        /// <summary>
        /// Configure the swagger
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Board Outlook Api",
                    Description = "Board Outlook Portal Api"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var filePath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                s.IncludeXmlComments(filePath);
            });
            return services;
        }
    }
}

