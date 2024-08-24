using System.Text.Json;

namespace DevicesMetricsGenerator;

public class Worker(ILogger<Worker> logger, ISensorStoreService sensorStoreService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var currentHour = 10;
        var currentMonth = 6;
        var simulator = new TemperatureSimulator(15.0, currentHour, currentMonth);
        var pollutionSimulator = new PollutionSimulator();
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var sensors = await sensorStoreService.GetSensorsAsync();
                var temperature = simulator.GetNextTemperature();
                foreach (var sensor in sensors)
                {
                    logger.LogInformation($"Temperature for Sensor {sensor.SensorId} at month {currentMonth}, hour {currentHour} : {temperature}");
                    var pollutionResult = pollutionSimulator.GenerateMetrics();
                    var serializedPollution = JsonSerializer.Serialize(pollutionResult);
                    logger.LogInformation($"Current Pollution for Sensor {sensor.SensorId} : {serializedPollution}");
                }
                
                currentHour = (currentHour + 1) % 24;
                //every 24h move to next month
                if (currentHour is 0)
                {
                    currentMonth = (currentMonth % 12) + 1;
                }
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}