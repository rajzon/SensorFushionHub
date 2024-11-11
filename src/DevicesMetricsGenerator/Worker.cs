using System.Text.Json;
using Contracts.DevicesMetricsGenerator;
using MassTransit;

namespace DevicesMetricsGenerator;

public class Worker(ILogger<Worker> logger, 
    ISensorStoreService sensorStoreService, 
    IBus bus,
    TimeProvider timeProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var currentHour = 10;
        var currentMonth = 6;
        var simulator = new TemperatureSimulator(15.0, currentHour, currentMonth, timeProvider);
        var pollutionSimulator = new PollutionSimulator(timeProvider);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    var sensors = await sensorStoreService.GetSensorsAsync();
                    var temperatureMetric = simulator.GetNextTemperature();
                    foreach (var sensor in sensors)
                    {
                        var sensorMetrics = new List<SensorMetric> { temperatureMetric };
                        logger.LogInformation(
                            $"Temperature for Sensor {sensor.SensorId} at month {currentMonth}, hour {currentHour} : {temperatureMetric.Value}");
                        var pollutionMetrics = pollutionSimulator.GenerateMetrics();
                        var serializedPollution = JsonSerializer.Serialize(pollutionMetrics);
                        logger.LogInformation(
                            $"Current Pollution for Sensor {sensor.SensorId} : {serializedPollution}");
                        
                        sensorMetrics.AddRange(pollutionMetrics);
                        await bus.Publish(new SensorAddedMetricsEvent(sensor.SensorId, sensorMetrics), stoppingToken);
                        await Task.Delay(50_000);
                    }

                    currentHour = (currentHour + 1) % 24;
                    //every 24h move to next month
                    if (currentHour is 0)
                    {
                        currentMonth = (currentMonth % 12) + 1;
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occured during worker execution");
            }
            
            await Task.Delay(1000, stoppingToken);
        }
    }
}