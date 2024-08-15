using System.Diagnostics;
using System.Reflection;
using Devices.API.Infrastructure.Abstract;
using Devices.API.Infrastructure.Telemetry;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Devices.API.Infrastructure;

internal sealed class ValidationFilter<T>(ILogger<ValidationFilter<T>> logger, ILoggableRequestTypeInfoCacheCacheAccessor cacheAccessor) 
    : IEndpointFilter where T : IBaseRequest
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argToValidate = context.GetArgument<T>(0);
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is null)
        {
            throw new InvalidOperationException($"No validator found for ({typeof(T).Name})");
        }

        var validationResult = await validator.ValidateAsync(argToValidate);
        if (validationResult.IsValid)
        {
            return await next(context);
        }
        
        var requestName = typeof(T).Name;
        var loggableAttribute = cacheAccessor.Cache.GetOrAdd(
            typeof(T),
            type => type.GetCustomAttributes<LoggableRequestAttribute>().FirstOrDefault());
        
        if (loggableAttribute is not null)
        {
            ValidationFailedAddActivityEvent(requestName, validationResult.Errors, true);
            logger.LogError("Validation failed for {AppRequestName} {@AppRequestValue}", requestName, argToValidate);
        }
        else
        {
            ValidationFailedAddActivityEvent(requestName, validationResult.Errors, false);
            logger.LogError("Validation failed for {AppRequestName}", requestName);
        }
        
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    private static void ValidationFailedAddActivityEvent(string requestName, List<ValidationFailure> failures, bool addTagForAttemptedValue)
    {
        foreach (var failure in failures)
        {
            var tags = new ActivityTagsCollection(new List<KeyValuePair<string, object?>>()
            {
                new(DiagnosticsNames.RequestName, requestName),
                new($"{DiagnosticsNames.Request}.failed_field", failure.PropertyName),
            });
            
            if (addTagForAttemptedValue)
            {
                tags.Add(new($"{DiagnosticsNames.Request}.failed_value", failure.AttemptedValue));
            }
            
            Activity.Current?.AddEvent(new ActivityEvent(
                "ValidationFailed",
                tags: tags));
        }
    }
}