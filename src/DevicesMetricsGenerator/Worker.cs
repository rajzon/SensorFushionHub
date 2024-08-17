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
        int initialHour = 10;
        var simulator = new TemperatureSimulator(15.0, initialHour, 6);
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation($"Temperature at hour {initialHour} : {simulator.GetNextTemperature()}");
                initialHour = (initialHour + 1) % 24;
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}