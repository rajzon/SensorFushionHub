using FluentResults;
using MediatR;

namespace Devices.API.Features.Sensors.GetSensor.Models;

public sealed record GetSensorQuery(string Id) : IRequest<Result<SensorDto>>;