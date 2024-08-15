using System.Reflection;
using Devices.API.Infrastructure.Abstract;
using FluentValidation;
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
            logger.LogError("Validation failed for {AppRequestName} {@AppRequestValue}", requestName, argToValidate);
        }
        else
        {
            logger.LogError("Validation failed for {AppRequestName}", requestName);
        }
        
        return Results.ValidationProblem(validationResult.ToDictionary());
    }
}