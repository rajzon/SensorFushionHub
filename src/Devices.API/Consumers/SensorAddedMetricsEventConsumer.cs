using System.Diagnostics;
using Contracts.DevicesMetricsGenerator;
using Devices.API.Infrastructure.Telemetry;
using MassTransit;
using NRedisStack;
using NRedisStack.DataTypes;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace Devices.API.Consumers;

public sealed class SensorAddedMetricsEventConsumer(
    ILogger<SensorAddedMetricsEventConsumer> logger,
    IMetricRepository metricRepository) : IConsumer<SensorAddedMetricsEvent>
{
    public async Task Consume(ConsumeContext<SensorAddedMetricsEvent> context)
    {
        using var activity = StartActivity(context.Message);
        //TODO add traces and metrics
        //This logs probably not needed if i will add Traces from MassTransit
        logger.LogInformation($"{nameof(SensorAddedMetricsEventConsumer)} started");
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
        logger.LogInformation($"{nameof(SensorAddedMetricsEventConsumer)} consumed");
    }
    
    private static Activity? StartActivity(SensorAddedMetricsEvent message)
    {
        return DiagnosticsConfig.Source.StartActivityWithTags(DiagnosticsNames.AddSensorMetric,
            new List<KeyValuePair<string, object?>>
            {
                new(DiagnosticsNames.SensorId, message.SensorId),
                //IMPORTANT ToList is required in order to treat each individual element in array as a value
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

public interface IMetricRepository
{
    bool IsKeyExist(string key);
    Task CreateKeyAsync(string key);
    Task<IReadOnlyList<TimeStamp>> AddAsync(IReadOnlyList<TsSensorMetric> metrics);
}

public class MetricRepository(IConnectionMultiplexer redis) : IMetricRepository
{
    public bool IsKeyExist(string key)
    {
        var db = redis.GetDatabase();
        return db.KeyExists(key);
    }
    
    public Task CreateKeyAsync(string key)
    {
        var db = redis.GetDatabase();
        var timeSeries = db.TS();
        return timeSeries.CreateAsync(key, new TsCreateParamsBuilder().build());
    }
    
    public Task<IReadOnlyList<TimeStamp>> AddAsync(IReadOnlyList<TsSensorMetric> metrics)
    {
        var db = redis.GetDatabase();
        var timeSeries = db.TS();
        return timeSeries.MAddAsync(metrics
            .Select(m => (m.Key, TimeStamp: new TimeStamp(m.CreatedDate), m.Value))
            .ToList()
        );
    }
}

//TODO move to Core
public record TsSensorMetric
{
    //TODO needed KeyPrefix?
    public string KeyPrefix { get; }
    public string Key { get; }
    public DateTime CreatedDate { get; }
    public double Value { get; }
    
    public TsSensorMetric(string sensorId, MetricType metricType, DateTime createdDate, double value)
    {
        Validate(createdDate);
        
        CreatedDate = createdDate;
        Value = value;
        KeyPrefix = metricType switch
        {
            MetricType.Temperature => "temperature",
            MetricType.Co2 => "co2",
            MetricType.No2 => "no2",
            MetricType.Pm10 => "pm10",
            MetricType.Pm2_5 => "pm2_5",
            MetricType.O3 => "o3",
            _ => throw new Exception($"Unknown type: {metricType}")
        };

        Key = $"{KeyPrefix}:{sensorId}";
    }

    private void Validate(DateTime createdDate)
    {
        if (createdDate.Kind is not DateTimeKind.Utc)
        {
            //TODO create custom exception
            throw new ArgumentException("Created date must be UTC.", nameof(createdDate));
        }
    }
}