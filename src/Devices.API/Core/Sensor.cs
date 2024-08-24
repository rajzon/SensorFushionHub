namespace Devices.API.Core;

public sealed class Sensor : Device
{
    public string Name { get; private set; }

    public Sensor(string name, DateTime createdDate) : base(createdDate)
    {
        if (createdDate.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException($"{nameof(CreatedDate)} must be UTC", nameof(createdDate));
        }
        
        Name = name;
    }
}