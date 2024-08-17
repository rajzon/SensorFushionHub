using DevicesMetricsGenerator.Core;

namespace DevicesMetricsGenerator.Infrastructure;

public interface ISensorRepository
{
    Task CreateAsync(Sensor sensor);
    Task<List<Sensor>> GetAllAsync();
}