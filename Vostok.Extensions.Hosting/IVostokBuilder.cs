using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting;

/// <summary>
/// Builder for configuring Vostok components.
/// </summary>
[PublicAPI]
public interface IVostokBuilder
{
    /// <summary>
    /// The host <see cref="IServiceCollection"/> where Vostok components are configured.
    /// </summary>
    IServiceCollection Services { get; }
}