using Vostok.Configuration.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Configuration;

internal sealed class VostokConfigurationMiscellaneousOptions
{
    public ICollection<Action<IConfigurationProvider, IConfigurationSource>> Customizations { get; } = [];
}