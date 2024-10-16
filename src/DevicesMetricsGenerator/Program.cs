using AsyncKeyedLock;
using DevicesMetricsGenerator;
using DevicesMetricsGenerator.Infrastructure;
using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<DevicesMetricsGeneratorDatabaseSettings>(
    builder.Configuration.GetSection("DevicesMetricsGeneratorDatabase"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetService<IOptions<DevicesMetricsGeneratorDatabaseSettings>>();
    return new MongoClient(settings!.Value.ConnectionString);
});
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetService<IOptions<DevicesMetricsGeneratorDatabaseSettings>>();
    return sp.GetRequiredService<IMongoClient>()
        .GetDatabase(settings!.Value.DatabaseName);
});
builder.Services.AddSingleton<ISensorRepository, SensorRepository>();
builder.Services.AddSingleton<ISensorStoreService, SensorStoreService>();
builder.Services.AddSingleton<AsyncKeyedLocker<string>>();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddMongoDbOutbox(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(5);
        o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
        o.UseBusOutbox();
                
        o.ClientFactory(sp => sp.GetRequiredService<IMongoClient>());
        o.DatabaseFactory(sp => sp.GetRequiredService<IMongoDatabase>());
    });
            
    busConfigurator.SetKebabCaseEndpointNameFormatter();

    busConfigurator.AddConsumer<SensorCreatedEventConsumer>();
    busConfigurator.AddConfigureEndpointsCallback((context, name, cfg) =>
    {
        cfg.UseMongoDbOutbox(context);
    });
    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(builder.Configuration["RabbitMq:Host"]!), h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"]!);
            h.Password(builder.Configuration["RabbitMq:Username"]!);
        });
        configurator.UseDelayedRedelivery(r => r.Intervals(
            TimeSpan.FromMinutes(5),
            TimeSpan.FromSeconds(15),
            TimeSpan.FromMinutes(30),
            TimeSpan.FromMinutes(60))
        );
        configurator.UseMessageRetry(r => r.Immediate(5));
        
        configurator.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();