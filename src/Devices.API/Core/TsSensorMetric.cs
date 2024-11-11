using Contracts.DevicesMetricsGenerator;
using Devices.API.Core.Exceptions;

namespace Devices.API.Core;

public record TsSensorMetric
{
    public string KeyPrefix { get; }
    public string Key { get; }
    public DateTime CreatedDate { get; }
    public double Value { get; }
    
    public TsSensorMetric(string sensorId, MetricType metricType, DateTime createdDate, double value)
    {
        Validate(createdDate);
        
        CreatedDate = createdDate;
        Value = value;
        KeyPrefix = metricType switch
        {
            MetricType.Temperature => "temperature",
            MetricType.Co2 => "co2",
            MetricType.No2 => "no2",
            MetricType.Pm10 => "pm10",
            MetricType.Pm2_5 => "pm2_5",
            MetricType.O3 => "o3",
            _ => throw new Exception($"Unknown type: {metricType}")
        };

        Key = $"{KeyPrefix}-{sensorId}";
    }

    private void Validate(DateTime createdDate)
    {
        if (createdDate.Kind is not DateTimeKind.Utc)
        {
            throw new UtcDateViolationException();
        }
    }
}