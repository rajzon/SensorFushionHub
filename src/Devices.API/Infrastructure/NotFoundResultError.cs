using FluentResults;

namespace Devices.API.Infrastructure;

internal class NotFoundResultError : Error
{
    protected NotFoundResultError()
    {
    }

    public NotFoundResultError(string message) : base(message)
    {
    }

    public NotFoundResultError(string message, IError causedBy) : base(message, causedBy)
    {
    }
}