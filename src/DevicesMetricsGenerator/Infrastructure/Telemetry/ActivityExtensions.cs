using System.Diagnostics;

namespace DevicesMetricsGenerator.Infrastructure.Telemetry;

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
}