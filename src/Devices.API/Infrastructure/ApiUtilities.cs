using FluentResults;

namespace Devices.API.Infrastructure;

internal static class ApiUtilities
{
    public static IResult HandleResult<T>(Result<T> result, string? createdStatusRoute = null)
    {
        if (result.IsFailed)
        {
            if (result.HasError<NotFoundResultError>())
            {
                return Results.NotFound(result.Errors);
            }
                
            return Results.Problem();
        }

        if (createdStatusRoute is not null)
        {
            return Results.Created(createdStatusRoute, result.Value);
        }
        
        return Results.Ok(result.Value);
    }
}