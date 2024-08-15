using System.Diagnostics;
using System.Reflection;
using Devices.API.Infrastructure.Abstract;
using MediatR;
using OpenTelemetry.Trace;

namespace Devices.API.Infrastructure;

internal sealed class ExceptionHandlerBehaviour<TRequest, TResponse>(ILogger<TRequest> logger, ILoggableRequestTypeInfoCacheCacheAccessor cacheAccessor) : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            var loggableAttribute = cacheAccessor.Cache.GetOrAdd(
                typeof(TRequest),
                type => type.GetCustomAttributes<LoggableRequestAttribute>().FirstOrDefault());

            if (loggableAttribute is not null)
            {
                logger.LogError("Exception occured for Request {AppRequestName} {@AppRequestValue}", requestName, request);
            }
            else
            {
                logger.LogError("Exception occured for Request {AppRequestName}", requestName);
            }
            
            Activity.Current?.SetStatus(ActivityStatusCode.Error);
            Activity.Current?.RecordException(ex, new TagList
            {
                { "app_request.name", requestName}
            });
            throw;
        }
    }
}