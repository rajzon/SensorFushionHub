using Devices.API.Features.Sensors.GetSensor.Models;
using Devices.API.Infrastructure;
using FluentResults;
using MediatR;

namespace Devices.API.Features.Sensors.GetSensors.Models;

[LoggableRequest]
public sealed record GetSensorsQuery() : IRequest<Result<List<SensorDto>>>;