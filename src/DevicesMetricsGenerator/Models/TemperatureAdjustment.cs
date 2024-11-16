namespace DevicesMetricsGenerator.Models;

//Make it readonly struct?
internal sealed record TemperatureAdjustment(double SeasonalAdjustment, double DiurnalAdjustment, double AdditionalAdjustment);