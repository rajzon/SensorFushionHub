using Devices.API.Infrastructure;
using FluentResults;
using MediatR;

namespace Devices.API.Features.Sensors.CreateSensor.Models;

[LoggableRequest]
public sealed record CreateSensorCommand(string Name) : IRequest<Result<CreatedSensorDto>>;