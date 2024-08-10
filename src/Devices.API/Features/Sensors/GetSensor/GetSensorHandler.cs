using Devices.API.Core;
using Devices.API.Features.Sensors.Abstract;
using Devices.API.Features.Sensors.GetSensor.Models;
using Devices.API.Infrastructure;
using FluentResults;
using Mapster;
using MediatR;

namespace Devices.API.Features.Sensors.GetSensor;

public sealed class GetSensorHandler(ISensorRepository sensorRepository)
    : IRequestHandler<GetSensorQuery, Result<SensorDto>>
{
    public async Task<Result<SensorDto>> Handle(GetSensorQuery request, CancellationToken cancellationToken)
    {
        var sensor = await sensorRepository.GetAsync(request.Id);
        if (sensor is null)
        {
            return Result.Fail(new NotFoundResultError(ErrorMessages.SensorNotFound.ErrorMessage)
                .WithMoreErrorDetails(ErrorMessages.SensorNotFound));
        }
        
        return Result.Ok(sensor.Adapt<SensorDto>());
    }
}