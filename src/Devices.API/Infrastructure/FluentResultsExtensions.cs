using Devices.API.Core;
using FluentResults;

namespace Devices.API.Infrastructure;

internal static class FluentResultsExtensions
{
    public const string MoreDetailsMetadataKey = "moreDetails";
    
    public static Error WithMoreErrorDetails(this Error error, MoreDetailsErrorModel moreDetailsErrorModel)
        => error.WithMetadata(MoreDetailsMetadataKey, moreDetailsErrorModel);
}