using Carter;
using Serilog;

namespace Devices.API.Startup;

internal static class WebApplicationExtensions
{
    public static WebApplication SetupMiddlewares(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });
            app.UseSwaggerUI();
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