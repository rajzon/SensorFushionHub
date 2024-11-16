namespace Contracts.DevicesMetricsGenerator;

//TODO pass enum or use different queues or exchanges per metric type
//devices can produce multiple metrics types data but also only one type.
public sealed record SensorAddedMetricsEvent(string SensorId, List<SensorMetric> Metrics);

public sealed record SensorMetric(MetricType Type, DateTime CreatedDate, double Value);

public enum MetricType
{
    Temperature = 1,
    Co2 = 2,
    No2 = 3,
    Pm10 = 4,
    Pm2_5 = 5,
    O3 = 6
}