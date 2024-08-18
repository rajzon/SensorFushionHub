using System.Diagnostics;
using Carter;
using Devices.API.Features.Sensors;
using Devices.API.Features.Sensors.Abstract;
using Devices.API.Infrastructure;
using Devices.API.Infrastructure.Abstract;
using Devices.API.Infrastructure.Telemetry;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Devices.API.Startup;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStartupServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomOpenTelemetry(configuration);
        services.AddCustomProblemDetails();
        services.AddCustomSwagger();
        services.ConfigureOptions(configuration);
        services.AddCustomMediatR();
        services.AddMongoDb();
        services.AddFluentValidation();
        services.AddRepositories();
        services.AddCache();
        services.AddCarter();
        services.AddJsonConfiguration();
        return services;
    }
    private static void AddCustomOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var otelCollectorUrl = configuration["Otel:Endpoint"];
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("Devices.API"))
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource(DiagnosticsConfig.Source.Name);
                //TODO add listener for RabbitMQ later
                if (otelCollectorUrl is not null)
                {
                    tracing.AddOtlpExporter(s => s.Endpoint = new Uri(otelCollectorUrl));
                }
            })
            .WithMetrics(metrics =>
            {
                metrics.AddMeter(DiagnosticsConfig.Meter.Name)
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddAspNetCoreInstrumentation()
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation();

                if (otelCollectorUrl is not null)
                {
                    metrics.AddOtlpExporter(s => s.Endpoint = new Uri(otelCollectorUrl));
                }
            });
    }
    
    private static void AddCustomProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(po => po.CustomizeProblemDetails = pc =>
        {
            //TODO Remove when upgraded to .NET 9 https://github.com/dotnet/aspnetcore/pull/54478#issuecomment-2007571828
            pc.ProblemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? pc.HttpContext.TraceIdentifier);
        });
    }

    private static void AddCustomSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Devices.API", Version = "v1" });
        });
    }
    
    private static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DevicesDatabaseSettings>(
            configuration.GetSection("DevicesDatabase"));
    }

    private static void AddCustomMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlerBehaviour<,>));
    }

    private static void AddMongoDb(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetService<IOptions<DevicesDatabaseSettings>>();
            return new MongoClient(settings!.Value.ConnectionString);
        });
    }

    private static void AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ISensorRepository, SensorRepository>();
    }

    private static void AddCache(this IServiceCollection services)
    {
        services
            .AddSingleton<ILoggableRequestTypeInfoCacheCacheAccessor, LoggableRequestTypeInfoCacheCacheAccessor>();
    }

    private static void AddJsonConfiguration(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });
    }
    
}