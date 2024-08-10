using Devices.API.Core;

namespace Devices.API.Features.Sensors.Abstract;

public interface ISensorRepository
{
    Task CreateAsync(Sensor sensor);
    Task<List<Sensor>> GetAllAsync();
    Task<Sensor?> GetAsync(string id);
}