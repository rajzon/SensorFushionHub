using System.Text.Json.Serialization;
using Devices.API.Core;
using Devices.API.Features.CreateSensor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
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

        var sampleTodos = new Todo[]
        {
            new(1, "Walk the dog"),
            new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
            new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
            new(4, "Clean the bathroom"),
            new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
        };

        var todosApi = app.MapGroup("/todos");
        todosApi.MapPost("/", async ([FromServices] ISensorRepository sensorRepository) =>
        {
            await sensorRepository.CreateAsync(new Sensor("Test Sensor Name"));
        }).WithOpenApi();
        
        todosApi.MapGet("/", async ([FromServices] ISensorRepository sensorRepository) =>
        {
            var sensors = await sensorRepository.GetAllAsync();
            
            return Results.Ok(sensors);
        }).WithOpenApi();
        // todosApi.MapGet("/{id}", (int id) =>
        //     sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        //         ? Results.Ok(todo)
        //         : Results.NotFound());

        app.Run();
    }
}

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
[JsonSerializable(typeof(List<Sensor>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}