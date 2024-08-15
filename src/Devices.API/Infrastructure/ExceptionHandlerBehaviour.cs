using System.Diagnostics;
using MediatR;
using OpenTelemetry.Trace;

namespace Devices.API.Infrastructure;

internal sealed class ExceptionHandlerBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            Activity.Current?.SetStatus(ActivityStatusCode.Error);
            Activity.Current?.RecordException(ex, new TagList
            {
                { "request.name", typeof(TRequest).Name}
            });
            throw;
        }
    }
}