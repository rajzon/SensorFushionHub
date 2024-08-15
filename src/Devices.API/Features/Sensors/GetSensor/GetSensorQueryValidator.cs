using Devices.API.Features.Sensors.GetSensor.Models;
using Devices.API.Infrastructure;
using FluentValidation;

namespace Devices.API.Features.Sensors.GetSensor;

public sealed class GetSensorQueryValidator : AbstractValidator<GetSensorQuery>
{
    public GetSensorQueryValidator()
    {
        RuleFor(x => x.Id).MustBeValidObjectId();
    }
}