using Devices.API.Core;
using Devices.API.Features.Sensors.Abstract;
using Devices.API.Features.Sensors.CreateSensor.Models;
using FluentResults;
using Mapster;
using MediatR;

namespace Devices.API.Features.Sensors.CreateSensor;

public sealed class CreateSensorHandler(ISensorRepository sensorRepository) : IRequestHandler<CreateSensorCommand, Result<CreatedSensorDto>>
{
    public async Task<Result<CreatedSensorDto>> Handle(CreateSensorCommand request, CancellationToken cancellationToken)
    {
        var sensor = new Sensor(request.Name);
        await sensorRepository.CreateAsync(sensor);

        return Result.Ok(sensor.Adapt<CreatedSensorDto>());
    }
}