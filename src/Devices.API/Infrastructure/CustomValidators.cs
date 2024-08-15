using FluentValidation;
using MongoDB.Bson;

namespace Devices.API.Infrastructure;

internal static class CustomValidators {
    public static IRuleBuilderOptions<T, string> MustBeValidObjectId<T>(this IRuleBuilder<T, string> ruleBuilder) {
        return ruleBuilder.Must(x => ObjectId.TryParse(x, out _)).WithMessage("{PropertyName} is not a valid ObjectId format");
    }
}