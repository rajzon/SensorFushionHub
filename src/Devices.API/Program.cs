using System.Diagnostics;
using System.Text.Json.Serialization;
using Carter;
using Devices.API.Core;
using Devices.API.Features.Sensors;
using Devices.API.Features.Sensors.Abstract;
using Devices.API.Features.Sensors.CreateSensor.Models;
using Devices.API.Features.Sensors.GetSensor.Models;
using Devices.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

namespace Devices.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddProblemDetails(po => po.CustomizeProblemDetails = pc =>
        {
            //TODO Remove when upgraded to .NET 9 https://github.com/dotnet/aspnetcore/pull/54478#issuecomment-2007571828
            pc.ProblemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? pc.HttpContext.TraceIdentifier);
        });
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
        builder.Services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetService<IOptions<DevicesDatabaseSettings>>();
            return new MongoClient(settings!.Value.ConnectionString);
        });
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

        if (app.Environment.IsProduction())
        {
            app.UseExceptionHandler("/exception");
        }
        
        app.MapCarter();
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
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}