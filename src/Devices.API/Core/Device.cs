namespace Devices.API.Core;

internal abstract class Device
{
    //TODO decide type of keys: Ulid?
    public Guid Id { get; }
}

internal sealed class Sensor : Device
{
    
}