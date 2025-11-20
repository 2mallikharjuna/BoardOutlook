using BoardOutlook.Api.App_start;
using Microsoft.Extensions.DependencyInjection;

namespace BoardOutlook.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();   
            builder.Services.ConfigureApiSettings(builder.Configuration);

            builder.Services.AddHttpClient();
            builder.Services.ConfigureSwagger(builder.Configuration);
            builder.Services.ConfigurePollySettings();
            builder.Services.AddDependencies(builder.Configuration);

            // Caching    
            builder.Services.AddMemoryCache();

            // Add health checks
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.AppApiMiddlewares();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}
