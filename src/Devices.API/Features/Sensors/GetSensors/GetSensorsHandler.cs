using Devices.API.Features.Sensors.Abstract;
using Devices.API.Features.Sensors.GetSensor.Models;
using Devices.API.Features.Sensors.GetSensors.Models;
using FluentResults;
using Mapster;
using MediatR;

namespace Devices.API.Features.Sensors.GetSensors;

public sealed class GetSensorsHandler(ISensorRepository sensorRepository) : IRequestHandler<GetSensorsQuery, Result<List<SensorDto>>>
{
    public async Task<Result<List<SensorDto>>> Handle(GetSensorsQuery request, CancellationToken cancellationToken)
    {
        var sensors = await sensorRepository.GetAllAsync();
        return Result.Ok(sensors.Adapt<List<SensorDto>>());
    }
}