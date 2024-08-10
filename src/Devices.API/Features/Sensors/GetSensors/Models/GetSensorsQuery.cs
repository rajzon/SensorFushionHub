using Devices.API.Features.Sensors.GetSensor.Models;
using FluentResults;
using MediatR;

namespace Devices.API.Features.Sensors.GetSensors.Models;

public sealed record GetSensorsQuery() : IRequest<Result<List<SensorDto>>>;