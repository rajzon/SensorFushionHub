namespace Devices.API.Core.Exceptions;

public class UtcDateViolationException() : Exception(MessageFormat)
{
    private const string MessageFormat = "Created date cannot be UTC.";
}