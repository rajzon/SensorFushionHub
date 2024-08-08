using System.Text.Json.Serialization;
using Carter;
using Devices.API.Core;
using Devices.API.Features.Sensors;
using Devices.API.Features.Sensors.Abstract;
using Devices.API.Features.Sensors.CreateSensor;
using Devices.API.Features.Sensors.CreateSensor.Models;
using Devices.API.Infrastructure;
using Microsoft.OpenApi.Models;

namespace Devices.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Devices.API", Version = "v1" });
        });
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });
        builder.Services.AddCarter();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        builder.Services.Configure<DevicesDatabaseSettings>(
            builder.Configuration.GetSection("DevicesDatabase"));
        builder.Services.AddSingleton<ISensorRepository, SensorRepository>();

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI();
        }
        
        app.MapCarter();
        app.Run();
    }
}

[JsonSerializable(typeof(List<Sensor>))]
[JsonSerializable(typeof(CreateSensorCommand))]
[JsonSerializable(typeof(SensorDto))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}