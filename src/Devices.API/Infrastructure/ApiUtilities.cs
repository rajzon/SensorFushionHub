using System.Net;
using Devices.API.Features.Sensors.GetSensor;
using FluentResults;

namespace Devices.API.Infrastructure;

internal static class ApiUtilities
{
    public static IResult HandleResult<T>(Result<T> result, string? createdRoute = null)
    {
        if (result.IsFailed)
        {
            if (result.HasError<NotFoundResultError>())
            {

                return Results.Problem(statusCode: (int)HttpStatusCode.NotFound, detail: SetDetailMessage(result), extensions: SetMoreDetails(result));
            }
                
            return Results.Problem( detail: SetDetailMessage(result), extensions: SetMoreDetails(result));
        }

        if (createdRoute is not null)
        {
            return Results.Created(createdRoute, result.Value);
        }
        
        return Results.Ok(result.Value);
    }

    private static string? SetDetailMessage<T>(Result<T> result)
    {
        return result.Errors.Count > 0 ? string.Join('\n', result.Errors.Select(e => e.Message)) : null;
    }

    private static Dictionary<string, object?>? SetMoreDetails<T>(Result<T> result)
    {
        var moreDetails = result.Errors.Select(e => e.Metadata)
            .Where(m => m.ContainsKey(FluentResultsExtensions.MoreDetailsMetadataKey))
            .Select(m => m[FluentResultsExtensions.MoreDetailsMetadataKey])
            .ToArray();
        
        return moreDetails.Length > 0 ? new Dictionary<string, object?>() { { "moreDetails", moreDetails } } : null;
    }
}