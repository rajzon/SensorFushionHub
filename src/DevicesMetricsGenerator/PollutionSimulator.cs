using Contracts.DevicesMetricsGenerator;

namespace DevicesMetricsGenerator;

internal sealed class PollutionSimulator(TimeProvider timeProvider)
{
    private static readonly Random Random = new();

    public List<SensorMetric> GenerateMetrics()
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        return
        [
            new SensorMetric(SensorType.Co2, now, GenerateRandomValue(400, 600)), // Typical CO2 levels in ppm
            new SensorMetric(SensorType.No2, now, GenerateRandomValue(10, 50)), // Typical NO2 levels in ppb
            new SensorMetric(SensorType.Pm10, now, GenerateRandomValue(20, 100)), // Typical PM10 levels in µg/m³
            new SensorMetric(SensorType.Pm2_5, now, GenerateRandomValue(10, 50)), // Typical PM2.5 levels in µg/m³
            new SensorMetric(SensorType.O3, now, GenerateRandomValue(20, 70)), // Typical O3 levels in ppb 
        ];
    }

    private double GenerateRandomValue(double minValue, double maxValue)
    {
        return Random.NextDouble() * (maxValue - minValue) + minValue;
    }
}