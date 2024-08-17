namespace DevicesMetricsGenerator.Models;

//Make it readonly struct?
internal sealed class PollutionMetric
{
    public double CO2 { get; set; }  // CO2 level in ppm (parts per million)
    public double NO2 { get; set; }  // NO2 level in ppb (parts per billion)
    public double PM10 { get; set; } // Particulate matter 10 micrometers or less in diameter
    public double PM2_5 { get; set; } // Particulate matter 2.5 micrometers or less in diameter
    public double O3 { get; set; }   // Ozone level in ppb
}