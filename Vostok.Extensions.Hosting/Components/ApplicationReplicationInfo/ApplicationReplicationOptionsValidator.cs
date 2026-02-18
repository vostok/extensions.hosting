using Microsoft.Extensions.Options;

namespace Vostok.Extensions.Hosting.Components.ApplicationReplicationInfo;

internal sealed class ApplicationReplicationOptionsValidator : IValidateOptions<ApplicationReplicationOptions>
{
    public ValidateOptionsResult Validate(string? _, ApplicationReplicationOptions options) =>
        // todo: return commented section after https://yt.skbkontur.ru/issue/INFRA-12410
        options.InstanceIndex < 0 /*|| options.InstanceIndex >= options.InstancesCount*/
            ? ValidateOptionsResult.Fail($"Instance index ({options.InstanceIndex}) not in range [0, {options.InstancesCount - 1}].")
            : ValidateOptionsResult.Success;
}