namespace Devices.API.Infrastructure.Telemetry;

internal static class DiagnosticsNames
{
    public const string CreateSensorName = "Creating Sensor";
    public const string AddSensorMetric = "Adding metric for Sensor";

    public const string SensorName = "sensor.name";
    public const string SensorId = "sensor.id";
    
    public const string MetricType = "metric.type";
    
    public const string Request = "app_request";
    public const string RequestName = $"{Request}.name";
}