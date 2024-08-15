using Devices.API.Infrastructure;
using FluentResults;
using MediatR;

namespace Devices.API.Features.Sensors.GetSensor.Models;

[LoggableRequest]
public sealed record GetSensorQuery(string Id) : IRequest<Result<SensorDto>>;