namespace Devices.API.Infrastructure;

internal sealed class DevicesDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string SensorsCollectionName { get; set; } = null!;
}