using System.Collections.Concurrent;
using AsyncKeyedLock;
using DevicesMetricsGenerator.Core;
using DevicesMetricsGenerator.Infrastructure;

namespace DevicesMetricsGenerator;

internal sealed class SensorStoreService(ISensorRepository sensorRepository, AsyncKeyedLocker<string> locker) : ISensorStoreService
{
    private ConcurrentBag<Sensor> _sensors { get; } = [];
    
    public async Task<ConcurrentBag<Sensor>> GetSensorsAsync()
    {
        if (_sensors.IsEmpty)
        {
            await InitializeCollectionIfEmpty();
        }
        
        return _sensors;
    }

    public async Task AddSensorAsync(Sensor sensor)
    {
        await InitializeCollectionIfEmpty();
        await sensorRepository.CreateAsync(sensor);
        _sensors.Add(sensor);
    }


    private async Task InitializeCollectionIfEmpty()
    {
        using (await locker.LockAsync(nameof(SensorStoreService)))
        {
            if (_sensors.IsEmpty)
            {
                var sensors = await sensorRepository.GetAllAsync();
                sensors.ForEach(s => _sensors.Add(s));
            }
        }
    }
}