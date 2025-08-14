using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Vostok.Clusterclient.Core.Topology;
using Vostok.Hercules.Client;
using Vostok.Hercules.Client.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Hercules;

/// <summary>
/// Builder for configuring <see cref="IHerculesSink"/>.
/// </summary>
[PublicAPI]
public interface IHerculesSinkBuilder
{
    /// <summary>
    /// The host <see cref="IServiceCollection"/> where <see cref="IHerculesSink"/> is configured.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Configures <see cref="HerculesSinkSettings"/>.
    /// </summary>
    /// <param name="configure">Delegate to configure <see cref="HerculesSinkSettings"/>.</param>
    /// <returns><see cref="IHerculesSinkBuilder"/> for chain method calls.</returns>
    IHerculesSinkBuilder Configure(Action<HerculesSinkSettings> configure);

    /// <summary>
    /// Sets default Hercules api key.
    /// </summary>
    /// <param name="apiKey">Default api key to set.</param>
    /// <returns><see cref="IHerculesSinkBuilder"/> for chain method calls.</returns>
    IHerculesSinkBuilder SetApiKey(string? apiKey);

    /// <summary>
    /// Sets provided <see cref="IClusterProvider"/> for Hercules topology.
    /// </summary>
    /// <param name="implementationFactory">The <see cref="IClusterProvider"/> implementation factory.</param>
    /// <returns><see cref="IHerculesSinkBuilder"/> for chain method calls.</returns>
    IHerculesSinkBuilder SetClusterProvider(Func<IServiceProvider, IClusterProvider?> implementationFactory);
}