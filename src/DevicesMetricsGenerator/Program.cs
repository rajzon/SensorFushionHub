using AsyncKeyedLock;
using DevicesMetricsGenerator;
using DevicesMetricsGenerator.Infrastructure;
using DevicesMetricsGenerator.Startup;
using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddStartupServices(builder.Configuration);

var host = builder.Build();
host.Run();