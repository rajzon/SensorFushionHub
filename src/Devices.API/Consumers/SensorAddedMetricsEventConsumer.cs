using System.Diagnostics;
using Contracts.DevicesMetricsGenerator;
using Devices.API.Core;
using Devices.API.Infrastructure.Telemetry;
using MassTransit;

namespace Devices.API.Consumers;

public sealed class SensorAddedMetricsEventConsumer(
    ILogger<SensorAddedMetricsEventConsumer> logger,
    IMetricRepository metricRepository) : IConsumer<SensorAddedMetricsEvent>
{
    public async Task Consume(ConsumeContext<SensorAddedMetricsEvent> context)
    {
        using var activity = StartActivity(context.Message);
        var tsToAdd = context.Message.Metrics.Select(m =>
            new TsSensorMetric(context.Message.SensorId, m.Type, m.CreatedDate, m.Value))
            .ToList();
        foreach (var ts in tsToAdd)
        {
            if (!metricRepository.IsKeyExist(ts.Key))
            {
                await metricRepository.CreateKeyAsync(ts.Key);
            }
        }
        await metricRepository.AddAsync(tsToAdd);
        AddSensorMetricsCreated(context.Message);
    }
    
    private static Activity? StartActivity(SensorAddedMetricsEvent message)
    {
        return DiagnosticsConfig.Source.StartActivityWithTags(DiagnosticsNames.AddSensorMetric,
            new List<KeyValuePair<string, object?>>
            {
                new(DiagnosticsNames.SensorId, message.SensorId),
                new(DiagnosticsNames.MetricType, string.Join(',', message.Metrics.Select(m => m.Type))),
            });
    }
    
    private static void AddSensorMetricsCreated(SensorAddedMetricsEvent message)
    {
        //TODO count globally how many metrics there are
        //TODO count globally how many metrics there are with distinguish between type - ex. 4 PM2_5, 1 PM10 etc.
        //TODO count Metrics per SensorId - add label for SensorId
        //TODO count Metrics per SensorId per Type - add label for SensorId and Type
        //TODO In Dashboard show info about current numbers(card) and histogram
        message.Metrics.ForEach(m =>
        {
            //TODO use multiple tags or create multiple metrics like CreatedMetrics per SensorId etc.
            DiagnosticsConfig.CreatedMetrics.Record(1, new TagList { { DiagnosticsNames.SensorId, message.SensorId }, { DiagnosticsNames.MetricType, m.Type } });
            DiagnosticsConfig.CreatedMetricsCount.Add(1, new TagList { { DiagnosticsNames.SensorId, message.SensorId }, { DiagnosticsNames.MetricType, m.Type } });
        });
        
    }
}