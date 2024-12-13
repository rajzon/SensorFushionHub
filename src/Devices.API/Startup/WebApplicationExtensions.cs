using Carter;
using Scalar.AspNetCore;
using Serilog;

namespace Devices.API.Startup;

internal static class WebApplicationExtensions
{
    public static WebApplication SetupMiddlewares(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }

        if (app.Environment.IsProduction())
        {
            app.UseExceptionHandler("/exception");
        }

        app.UseSerilogRequestLogging();
        app.MapCarter();

        return app;
    }
}