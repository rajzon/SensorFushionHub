using System.Diagnostics;
using Contracts;
using Contracts.DevicesAPI;
using Devices.API.Core;
using Devices.API.Features.Sensors.Abstract;
using Devices.API.Features.Sensors.CreateSensor.Models;
using Devices.API.Infrastructure.Telemetry;
using FluentResults;
using Mapster;
using MassTransit;
using MassTransit.MongoDbIntegration;
using MediatR;

namespace Devices.API.Features.Sensors.CreateSensor;

public sealed class CreateSensorHandler(ISensorRepository sensorRepository,
    IPublishEndpoint publishEndpoint,
    TimeProvider timeProvider,
    MongoDbContext massTransitMongoDbContext) : IRequestHandler<CreateSensorCommand, Result<CreatedSensorDto>>
{
    public async Task<Result<CreatedSensorDto>> Handle(CreateSensorCommand request, CancellationToken cancellationToken)
    {
        using var activity = StartActivity(request);
        var session = await massTransitMongoDbContext.StartSession(cancellationToken);
        await massTransitMongoDbContext.BeginTransaction(cancellationToken);
        
        try
        {
            var sensor = new Sensor(request.Name, timeProvider.GetUtcNow().UtcDateTime);
            await sensorRepository.CreateAsync(sensor, session);
            await publishEndpoint.Publish(new SensorCreatedEvent(sensor.Id));

            await session.CommitTransactionAsync(cancellationToken);
            AddCreatedSensorMetrics();
            return Result.Ok(sensor.Adapt<CreatedSensorDto>());
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static Activity? StartActivity(CreateSensorCommand request)
    {
        return DiagnosticsConfig.Source.StartActivityWithTags(DiagnosticsNames.CreateSensorName,
            new List<KeyValuePair<string, object?>>
            {
                new(DiagnosticsNames.SensorName, request.Name)
            });
    }
    
    private static void AddCreatedSensorMetrics()
    {
        DiagnosticsConfig.CreatedSensors.Record(1);
        DiagnosticsConfig.CreatedSensorsCount.Add(1);
    }
}