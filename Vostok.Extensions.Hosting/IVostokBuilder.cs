using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace Vostok.Extensions.Hosting;

/// <summary>
/// Builder for configuring Vostok components.
/// </summary>
[PublicAPI]
public interface IVostokBuilder
{
    IServiceCollection Services { get; }
}