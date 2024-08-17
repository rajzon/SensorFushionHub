namespace DevicesMetricsGenerator.Infrastructure;

internal sealed class DevicesMetricsGeneratorDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string SensorsCollectionName { get; set; } = null!;
}