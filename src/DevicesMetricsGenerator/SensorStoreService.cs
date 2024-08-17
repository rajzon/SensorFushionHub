using DevicesMetricsGenerator.Core;
using DevicesMetricsGenerator.Infrastructure;

namespace DevicesMetricsGenerator;

internal sealed class SensorStoreService(ISensorRepository sensorRepository) : ISensorStoreService
{
    //TODO Update internal collection when SensorId will come from DevicesAPI via Rabbit
    //TODO Consider Thread-safety
    private List<Sensor> _sensors { get; } = [];
    
    public Task<List<Sensor>> GetSensors()
    {
        return _sensors.Count is 0 ? sensorRepository.GetAllAsync() : Task.FromResult(_sensors);
    }
}