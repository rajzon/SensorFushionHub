using System.Collections.Concurrent;

namespace Devices.API.Infrastructure.Abstract;

public interface ILoggableRequestTypeInfoCacheCacheAccessor 
{
    ConcurrentDictionary<Type, LoggableRequestAttribute?> Cache { get; }
}