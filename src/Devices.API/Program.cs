using System.Text.Json;
using System.Text.Json.Serialization;
using Contracts.DevicesAPI;
using Contracts.DevicesMetricsGenerator;
using Devices.API.Core;
using Devices.API.Features.Sensors.CreateSensor.Models;
using Devices.API.Features.Sensors.GetSensor.Models;
using Devices.API.Startup;
using Microsoft.AspNetCore.Mvc;

namespace Devices.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);
        builder.Host.ConfigureHostBuilder();
        builder.Services.AddStartupServices(builder.Configuration);
        
        var app = builder.Build();
        app.SetupMiddlewares();
        app.Run();
    }
}

[JsonSerializable(typeof(List<SensorDto>))]
[JsonSerializable(typeof(CreateSensorCommand))]
[JsonSerializable(typeof(CreatedSensorDto))]
[JsonSerializable(typeof(SensorDto))]
[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(object[]))]
[JsonSerializable(typeof(MoreDetailsErrorModel))]
[JsonSerializable(typeof(SensorCreatedEvent))]
[JsonSerializable(typeof(SensorAddedMetricsEvent))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(IEnumerable<KeyValuePair<string, object>>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}