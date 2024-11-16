using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Devices.API.Infrastructure.Telemetry;

internal static class DiagnosticsConfig
{
    private const string SourceName = "devices-api";
    public static ActivitySource Source = new(SourceName);
    
    
    public static Meter Meter = new(SourceName);
    public static Histogram<long> CreatedSensors = Meter.CreateHistogram<long>("created.sensors");
    public static Counter<long> CreatedSensorsCount = Meter.CreateCounter<long>("created.sensors.count");
    public static Histogram<long> CreatedMetrics = Meter.CreateHistogram<long>("created.metrics");
    public static Counter<long> CreatedMetricsCount = Meter.CreateCounter<long>("created.metrics.count");
}