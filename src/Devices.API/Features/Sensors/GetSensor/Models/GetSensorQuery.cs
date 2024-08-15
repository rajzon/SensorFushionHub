using Devices.API.Infrastructure;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Devices.API.Features.Sensors.GetSensor.Models;

[LoggableRequest]
public sealed record GetSensorQuery([FromRoute] string Id) : IRequest<Result<SensorDto>>;