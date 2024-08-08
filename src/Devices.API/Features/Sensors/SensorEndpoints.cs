using Carter;
using Devices.API.Core;
using Devices.API.Features.Sensors.CreateSensor;
using Microsoft.AspNetCore.Mvc;

namespace Devices.API.Features.Sensors;

public sealed class SensorEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/sensors").WithTags(nameof(SensorEndpoints));
        group.MapPost("/", async ([FromServices] ISensorRepository sensorRepository) =>
        {
            await sensorRepository.CreateAsync(new Sensor("Test Sensor Name"));
        }).WithOpenApi();
        
        group.MapGet("/", async ([FromServices] ISensorRepository sensorRepository) =>
        {
            var sensors = await sensorRepository.GetAllAsync();
            
            return Results.Ok(sensors);
        }).WithOpenApi();
    }
}