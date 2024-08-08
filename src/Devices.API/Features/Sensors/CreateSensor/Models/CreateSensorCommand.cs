using MediatR;

namespace Devices.API.Features.Sensors.CreateSensor.Models;

public sealed record CreateSensorCommand(string Name) : IRequest<SensorDto>;