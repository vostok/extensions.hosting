using Microsoft.Extensions.Options;

namespace Vostok.Extensions.Hosting.Components.ApplicationIdentity;

internal sealed class ApplicationIdentityOptionsValidator : IValidateOptions<ApplicationIdentityOptions>
{
    public ValidateOptionsResult Validate(string? _, ApplicationIdentityOptions options)
    {
        var missingFields = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Project))
            missingFields.Add(nameof(options.Project));
        if (string.IsNullOrWhiteSpace(options.Environment))
            missingFields.Add(nameof(options.Environment));
        if (string.IsNullOrWhiteSpace(options.Application))
            missingFields.Add(nameof(options.Application));
        if (string.IsNullOrWhiteSpace(options.Instance))
            missingFields.Add(nameof(options.Instance));

        if (missingFields.Count == 0)
            return ValidateOptionsResult.Success;

        return ValidateOptionsResult.Fail(
            $"Some of the Vostok application identity required fields have not been specified: '{string.Join(", ", missingFields)}'.");
    }
}