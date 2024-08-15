namespace Devices.API.Infrastructure;

/// <summary>
/// Should be used alongside IRequest
/// requests that are safe to be logged(do not have any security risky info inside payload)
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class LoggableRequestAttribute : Attribute;