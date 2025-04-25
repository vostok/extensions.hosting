using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Vostok.Extensions.AspNetCore.Helpers;
using Vostok.Extensions.AspNetCore.Middlewares;
using Vostok.Extensions.AspNetCore.Middlewares.PingApi;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.AspNetCore.HostedServices;

internal sealed class VostokAspNetCoreHostedService(
    IServer server,
    IOptions<VostokMiddlewaresOptions> middlewaresOptions,
    IConfiguration configuration,
    IServiceProvider provider,
    ILogger<VostokAspNetCoreHostedService> logger) : IHostedLifecycleService
{
    private Uri? _serviceBeaconUri;

    // Called before Kestrel started.
    public Task StartingAsync(CancellationToken cancellationToken)
    {
        SynchronizeAddressesWithServiceBeacon();

        return Task.CompletedTask;
    }

    // Called after Kestrel started.
    public async Task StartedAsync(CancellationToken cancellationToken)
    {
        if (!middlewaresOptions.Value.IsEnabled<PingApiMiddleware>())
            return;

        var warmupUrl = _serviceBeaconUri;

        if (warmupUrl is null)
        {
            if (GetServerAddresses()?.FirstOrDefault() is not { } fallbackAddress || !TryParseUrl(fallbackAddress, out var url))
                return;

            warmupUrl = url;
        }

        await MiddlewaresWarmup.WarmupPingApi(warmupUrl);
    }

    private void SynchronizeAddressesWithServiceBeacon()
    {
        var addresses = GetServerAddresses();
        var urls = configuration[WebHostDefaults.ServerUrlsKey]?.Split(';', StringSplitOptions.RemoveEmptyEntries);

        var serviceBeacon = provider.GetService<IServiceBeacon>();
        if (serviceBeacon is null) // todo: serviceBeacon is not ServiceBeacon
            return;

        if (serviceBeacon.ReplicaInfo.TryGetUrl(out var serviceBeaconUrl))
        {
            if (addresses == null)
                throw new Exception($"Service beacon url '{serviceBeaconUrl}' is incompatible with Generic host.");

            if (!HasAddress(addresses, serviceBeaconUrl) && !HasAddress(urls, serviceBeaconUrl))
                addresses.Add($"{serviceBeaconUrl.Scheme}://*:{serviceBeaconUrl.Port}/");

            logger.LogInformation("Using url provided in Service beacon: '{Url}'.", serviceBeaconUrl);
            _serviceBeaconUri = serviceBeaconUrl;

            return;
        }

        if (addresses?.Any() == true || urls?.Any() == true)
            throw new NotImplementedException("Dynamic configuration of Service beacon is not currently supported. Please configure port explicitly during Vostok environment setup.");
    }

    private ICollection<string>? GetServerAddresses() => server.Features.Get<IServerAddressesFeature>()?.Addresses;

    private static bool HasAddress(IEnumerable<string>? urls, Uri expectedUrl)
    {
        if (urls == null)
            return false;

        foreach (var url in urls)
        {
            if (TryParseUrl(url, out var parsed) && parsed.Port == expectedUrl.Port && parsed.Scheme == expectedUrl.Scheme)
                return true;
        }

        return false;
    }

    private static bool TryParseUrl(string url, [NotNullWhen(true)] out Uri? parsed)
    {
        url = url.Replace("://*:", "://localhost:");
        return Uri.TryCreate(url, UriKind.Absolute, out parsed);
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}