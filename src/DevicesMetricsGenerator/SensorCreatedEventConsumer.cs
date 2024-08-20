using Contracts.DevicesAPI;
using MassTransit;

namespace DevicesMetricsGenerator;

public sealed class SensorCreatedEventConsumer(ILogger<SensorCreatedEventConsumer> logger) : IConsumer<SensorCreatedEvent>
{
    public Task Consume(ConsumeContext<SensorCreatedEvent> context)
    {
        logger.LogInformation("Sensor Created Event Consumer");
        throw new Exception("test");
        return Task.CompletedTask;
    }
}