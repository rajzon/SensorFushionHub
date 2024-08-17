using DevicesMetricsGenerator.Models;

namespace DevicesMetricsGenerator;

internal sealed class PollutionSimulator
{
    private static readonly Random Random = new();

    public PollutionMetric GenerateMetrics()
    {
        return new PollutionMetric
        {
            CO2 = GenerateRandomValue(400, 600),    // Typical CO2 levels in ppm
            NO2 = GenerateRandomValue(10, 50),      // Typical NO2 levels in ppb
            PM10 = GenerateRandomValue(20, 100),    // Typical PM10 levels in µg/m³
            PM2_5 = GenerateRandomValue(10, 50),    // Typical PM2.5 levels in µg/m³
            O3 = GenerateRandomValue(20, 70)        // Typical O3 levels in ppb
        };
    }

    private double GenerateRandomValue(double minValue, double maxValue)
    {
        return Random.NextDouble() * (maxValue - minValue) + minValue;
    }
}