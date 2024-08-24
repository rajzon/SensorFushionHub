using Contracts.DevicesAPI;
using DevicesMetricsGenerator.Core;
using MassTransit;

namespace DevicesMetricsGenerator;

public sealed class SensorCreatedEventConsumer(ILogger<SensorCreatedEventConsumer> logger, ISensorStoreService sensorStoreService) : IConsumer<SensorCreatedEvent>
{
    public async Task Consume(ConsumeContext<SensorCreatedEvent> context)
    {
        //TODO add traces and metrics
        //This logs probably not needed if i will add Traces from MassTransit
        logger.LogInformation($"{nameof(SensorCreatedEventConsumer)} started");
        await Task.Delay(1000);
        await sensorStoreService.AddSensorAsync(new Sensor(context.Message.SensorId));
        
        logger.LogInformation($"{nameof(SensorCreatedEventConsumer)} consumed");
        return;
    }
}