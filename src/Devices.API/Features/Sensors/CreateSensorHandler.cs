using Devices.API.Core;
using Devices.API.Features.Sensors.Abstract;
using Devices.API.Features.Sensors.CreateSensor.Models;
using Mapster;
using MediatR;

namespace Devices.API.Features.Sensors;

public sealed class CreateSensorHandler(ISensorRepository sensorRepository) : IRequestHandler<CreateSensorCommand, SensorDto>
{
    public async Task<SensorDto> Handle(CreateSensorCommand request, CancellationToken cancellationToken)
    {
        var sensor = new Sensor(request.Name);
        await sensorRepository.CreateAsync(sensor);

        return sensor.Adapt<SensorDto>();
    }
}