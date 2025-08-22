using JetBrains.Annotations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting.Components.Configuration;

/// <summary>
/// Builder for configuring Vostok configuration components: <see cref="IConfigurationSource"/> and <see cref="IConfigurationProvider"/>.
/// </summary>
[PublicAPI]
public interface IVostokConfigurationBuilder
{
    /// <summary>
    /// The host <see cref="IServiceCollection"/> where Vostok configuration components are configured.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Registers settings of type <typeparamref name="TSettings"/> in <see cref="IConfigurationProvider"/>.
    /// </summary>
    /// <param name="scopes">Optional scopes for settings.</param>
    /// <typeparam name="TSettings">Type of settings.</typeparam>
    /// <returns><see cref="IVostokConfigurationBuilder"/> for chain method calls.</returns>
    IVostokConfigurationBuilder SetupSourceFor<TSettings>(params string[]? scopes);
}