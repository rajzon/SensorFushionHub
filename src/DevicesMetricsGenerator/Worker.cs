using System.Text.Json;

namespace DevicesMetricsGenerator;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int currentHour = 10;
        int currentMonth = 6;
        var simulator = new TemperatureSimulator(15.0, currentHour, currentMonth);
        var pollutionSimulator = new PollutionSimulator();
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation($"Temperature at month {currentMonth}, hour {currentHour} : {simulator.GetNextTemperature()}");
                currentHour = (currentHour + 1) % 24;
                //every 24h move to next month
                if (currentHour is 0)
                {
                    currentMonth = (currentMonth % 12) + 1;
                }
                var pollutionResult = pollutionSimulator.GenerateMetrics();
                var serializedPollution = JsonSerializer.Serialize(pollutionResult);
                _logger.LogInformation($"Current Pollution : {serializedPollution}");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}