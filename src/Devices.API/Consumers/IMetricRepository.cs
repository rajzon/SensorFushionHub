using Devices.API.Core;
using NRedisStack.DataTypes;

namespace Devices.API.Consumers;
//TODO move somewhere
public interface IMetricRepository
{
    bool IsKeyExist(string key);
    Task CreateKeyAsync(string key);
    Task<IReadOnlyList<TimeStamp>> AddAsync(IReadOnlyList<TsSensorMetric> metrics);
}