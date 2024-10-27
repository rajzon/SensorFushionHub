using System.Text;
using AsyncKeyedLock;
using DevicesMetricsGenerator.Infrastructure;
using DevicesMetricsGenerator.Infrastructure.Telemetry;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DevicesMetricsGenerator.Startup;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStartupServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<Worker>();
        
        services.AddCustomOpenTelemetry(configuration);
        services.ConfigureOptions(configuration);
        services.AddMongoDb();
        services.AddRepositories();
        services.AddServices();
        services.AddCache();
        services.AddCustomMassTransit(configuration);
        services.AddSingleton(TimeProvider.System);
        return services;
    }
    private static void AddCustomOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var otelCollectorUrl = configuration["Otel:Endpoint"];
        var auth64 = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{configuration["Otel:Username"]}:{configuration["Otel:Password"]}"));
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("DevicesMetricsGenerator"))
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource(DiagnosticsConfig.Source.Name)
                    .AddSource(DiagnosticHeaders.DefaultListenerName);
                if (otelCollectorUrl is not null)
                {
                    tracing.AddOtlpExporter(s =>
                    {
                        s.Endpoint = new Uri(otelCollectorUrl);
                        s.Headers = "Authorization=Basic " + auth64;
                    });
                }
            })
            .WithMetrics(metrics =>
            {
                metrics.AddMeter(DiagnosticsConfig.Meter.Name)
                    .AddMeter(InstrumentationOptions.MeterName)
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddAspNetCoreInstrumentation()
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation();

                if (otelCollectorUrl is not null)
                {
                    metrics.AddOtlpExporter(s =>
                    {
                        s.Endpoint = new Uri(otelCollectorUrl);
                        s.Headers = "Authorization=Basic " + auth64;
                    });
                }
            });
    }
    
    private static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DevicesMetricsGeneratorDatabaseSettings>(
            configuration.GetSection("DevicesMetricsGeneratorDatabase"));
    }

    private static void AddMongoDb(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetService<IOptions<DevicesMetricsGeneratorDatabaseSettings>>();
            return new MongoClient(settings!.Value.ConnectionString);
        });
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var settings = sp.GetService<IOptions<DevicesMetricsGeneratorDatabaseSettings>>();
            return sp.GetRequiredService<IMongoClient>()
                .GetDatabase(settings!.Value.DatabaseName);
        });
    }
    
    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ISensorRepository, SensorRepository>();
    }
    
    private static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<ISensorStoreService, SensorStoreService>();
    }
    
    private static void AddCache(this IServiceCollection services)
    {
        services.AddSingleton<AsyncKeyedLocker<string>>();
    }
    
    private static void AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfigurator =>
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
                configurator.Host(new Uri(configuration["RabbitMq:Host"]!), h =>
                {
                    h.Username(configuration["RabbitMq:Username"]!);
                    h.Password(configuration["RabbitMq:Username"]!);
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
    }
}