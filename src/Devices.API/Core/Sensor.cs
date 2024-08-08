namespace Devices.API.Core;

public sealed class Sensor : Device
{
    public string Name { get; private set; }

    public Sensor(string name)
    {
        Name = name;
    }
}