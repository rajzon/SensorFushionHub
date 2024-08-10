namespace Devices.API.Core;

internal static class ErrorMessages
{
    public static readonly MoreDetailsErrorModel SensorNotFound = new("{TODO_ERR_CODE}", "Sensor not found");
}

internal record MoreDetailsErrorModel(string ErrorCode, string ErrorMessage);