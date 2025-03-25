using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Builder;
using Vostok.Extensions.Hosting.Components.Logging.Substitution;
using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Helpers;

internal sealed class SubstitutionsValidator(IServiceProvider provider)
    : IValidateOptions<SubstitutionsValidator.SubstitutionMarker>
{
    public ValidateOptionsResult Validate(string? _, SubstitutionMarker __)
    {
        SubstituteLogging();

        return ValidateOptionsResult.Skip;
    }

    private void SubstituteLogging()
    {
        var substitutableLog = provider.GetService<ILog>() as SubstitutableLog;
        var actualLog = provider.GetKeyedService<ILog>(VostokBuilderTelemetryExtensions.ActualLogKey);

        if (substitutableLog is not null && actualLog is not null)
            substitutableLog.SubstituteWith(actualLog);
    }

    public sealed class SubstitutionMarker;
}