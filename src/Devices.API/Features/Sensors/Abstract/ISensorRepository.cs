using Devices.API.Core;
using MongoDB.Driver;

namespace Devices.API.Features.Sensors.Abstract;

public interface ISensorRepository
{
    Task CreateAsync(Sensor sensor, IClientSessionHandle session);
    Task<List<Sensor>> GetAllAsync();
    Task<Sensor?> GetAsync(string id);
}