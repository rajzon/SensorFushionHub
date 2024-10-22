using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace DevicesMetricsGenerator.Infrastructure.Telemetry;

internal static class DiagnosticsConfig
{
    private const string SourceName = "devices-metrics-generator";
    public static ActivitySource Source = new(SourceName);
    
    public static Meter Meter = new(SourceName);
}