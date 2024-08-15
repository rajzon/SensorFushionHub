using System.Diagnostics;

namespace Devices.API.Infrastructure.Telemetry;

internal static class ActivityExtensions
{
    public static Activity? StartActivityWithTags(this ActivitySource source, string name,
        List<KeyValuePair<string, object?>> tags)
    {
        return source.StartActivity(name,
            ActivityKind.Internal,
            Activity.Current?.Context ?? new ActivityContext(),
            tags);
    }

    public static void EnrichWithRequest<TRequest>(this Activity activity)
    {
        activity.SetTag(DiagnosticsNames.RequestName, typeof(TRequest).Name);
    }
}