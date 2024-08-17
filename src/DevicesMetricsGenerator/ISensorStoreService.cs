using DevicesMetricsGenerator.Core;

namespace DevicesMetricsGenerator;

public interface ISensorStoreService
{
    Task<List<Sensor>> GetSensors();
}