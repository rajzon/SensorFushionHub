using System.Net;
using Carter;
using Devices.API.Features.Sensors.CreateSensor.Models;
using Devices.API.Features.Sensors.GetSensor.Models;
using Devices.API.Features.Sensors.GetSensors.Models;
using Devices.API.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Devices.API.Features.Sensors;

public sealed class SensorEndpoints : ICarterModule
{
    private const string BaseUrl = "api/sensors";
    
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(BaseUrl).WithTags(nameof(SensorEndpoints));
        group.MapPost("/", async ([FromBody] CreateSensorCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return ApiUtilities.HandleResult(result, createdRoute: $"{BaseUrl}/{result.Value?.Id}");
        }).WithOpenApi()
            .Produces<CreatedSensorDto>((int)HttpStatusCode.Created);
        
        group.MapGet("/", async (IMediator mediator) 
            => ApiUtilities.HandleResult(await mediator.Send(new GetSensorsQuery())))
                .WithOpenApi()
                .Produces<List<SensorDto>>();
        
        group.MapGet("/{id}", async ([AsParameters] GetSensorQuery query, IMediator mediator) 
            => ApiUtilities.HandleResult( await mediator.Send(query)))
                .AddEndpointFilter<ValidationFilter<GetSensorQuery>>()
                .WithOpenApi()
                .Produces<SensorDto>();
    }
}