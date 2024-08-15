using Devices.API.Core;
using Devices.API.Features.Sensors.Abstract;
using Devices.API.Features.Sensors.CreateSensor.Models;
using Devices.API.Infrastructure.Telemetry;
using FluentResults;
using Mapster;
using MediatR;

namespace Devices.API.Features.Sensors.CreateSensor;

public sealed class CreateSensorHandler(ISensorRepository sensorRepository) : IRequestHandler<CreateSensorCommand, Result<CreatedSensorDto>>
{
    public async Task<Result<CreatedSensorDto>> Handle(CreateSensorCommand request, CancellationToken cancellationToken)
    {
        using var activity = DiagnosticsConfig.Source.StartActivityWithTags(DiagnosticsNames.CreateSensorName,
            new List<KeyValuePair<string, object?>>
            {
                new(DiagnosticsNames.SensorName, request.Name)
            });
        
        var sensor = new Sensor(request.Name);
        await sensorRepository.CreateAsync(sensor);
        
        
        AddCreatedSensorMetrics();
        return Result.Ok(sensor.Adapt<CreatedSensorDto>());
    }
    
    
    private static void AddCreatedSensorMetrics()
    {
        DiagnosticsConfig.CreatedSensors.Record(1);
        DiagnosticsConfig.CreatedSensorsCount.Add(1);
    }
}