using System.Collections.Concurrent;
using Devices.API.Infrastructure.Abstract;

namespace Devices.API.Infrastructure;

internal sealed class LoggableRequestTypeInfoCacheCacheAccessor : ILoggableRequestTypeInfoCacheCacheAccessor
{
    public ConcurrentDictionary<Type, LoggableRequestAttribute?> Cache { get; } = new();
}