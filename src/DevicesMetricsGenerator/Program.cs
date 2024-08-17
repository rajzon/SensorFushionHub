using DevicesMetricsGenerator;
using DevicesMetricsGenerator.Infrastructure;
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
builder.Services.AddSingleton<ISensorRepository, SensorRepository>();
builder.Services.AddSingleton<ISensorStoreService, SensorStoreService>();

var host = builder.Build();
host.Run();