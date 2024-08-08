using Carter;
using Devices.API.Features.Sensors.Abstract;
using Devices.API.Features.Sensors.CreateSensor.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Devices.API.Features.Sensors;

public sealed class SensorEndpoints : ICarterModule
{
    private const string BaseUrl = "api/sensors";
    
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(BaseUrl).WithTags(nameof(SensorEndpoints));
        group.MapPost("/", async ([FromBody] CreateSensorCommand command, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            
            return Results.Created($"{BaseUrl}/{result.Id}", result);
        }).WithOpenApi();
        
        group.MapGet("/", async ([FromServices] ISensorRepository sensorRepository) =>
        {
            var sensors = await sensorRepository.GetAllAsync();
            
            return Results.Ok(sensors);
        }).WithOpenApi();
        
        group.MapGet("/{id}", async (string id, [FromServices] ISensorRepository sensorRepository) =>
        {
            var sensor = await sensorRepository.GetAsync(id);
            
            return Results.Ok(sensor);
        }).WithOpenApi();
    }
}