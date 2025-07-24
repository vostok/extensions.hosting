using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Clusterclient.Core.Topology;
using Vostok.Hercules.Client;

namespace Vostok.Extensions.Hosting.Components.Hercules;

[PublicAPI]
public interface IHerculesSinkBuilder
{
    IServiceCollection Services { get; }

    IHerculesSinkBuilder Configure(Action<HerculesSinkSettings> configure);
    IHerculesSinkBuilder SetApiKey(string? apiKey);
    IHerculesSinkBuilder SetClusterProvider(Func<IServiceProvider, IClusterProvider?> implementationFactory);
}