using System.Collections.Concurrent;
using DevicesMetricsGenerator.Core;

namespace DevicesMetricsGenerator;

public interface ISensorStoreService
{
    Task<ConcurrentBag<Sensor>> GetSensorsAsync();
    Task AddSensorAsync(Sensor sensor);
}