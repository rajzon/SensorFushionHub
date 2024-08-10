using System.Diagnostics;
using Carter;
using Microsoft.AspNetCore.Diagnostics;

namespace Devices.API.Infrastructure;

public sealed class ExceptionHandlerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/exception", (HttpContext httpContext, ILogger<ExceptionHandlerEndpoint> logger) =>
        {
            var exception = httpContext.Features.Get<IExceptionHandlerFeature>()!.Error;
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            
            logger.LogError(exception.GetBaseException(), "Exception occured for traceId {traceId}", traceId);
            return Results.Problem();
        });
    }
}