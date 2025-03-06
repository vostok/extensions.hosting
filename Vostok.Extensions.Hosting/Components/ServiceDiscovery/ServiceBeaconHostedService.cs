using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Vostok.ServiceDiscovery;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

internal sealed class ServiceBeaconHostedService(
    IServiceBeacon serviceBeacon,
    IOptions<ServiceBeaconConfigurationOptions> options,
    ILogger<ServiceBeaconHostedService> logger) : IHostedLifecycleService
{
    private readonly ServiceBeaconConfigurationOptions _options = options.Value;

    public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    // Called after Kestrel started.
    public Task StartedAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting..");

        serviceBeacon.Start();
        WaitForServiceBeaconRegistration();

        return Task.CompletedTask;
    }

    // Called before Kestrel stopped
    public Task StoppingAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping..");

        serviceBeacon.Stop();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void WaitForServiceBeaconRegistration()
    {
        if (!_options.WaitRegistration)
            return;
        if (serviceBeacon is not ServiceBeacon actualBeacon)
            return;

        if (!actualBeacon.WaitForInitialRegistrationAsync().Wait(_options.RegistrationWaitTimeout))
            throw new Exception($"Service beacon hasn't registered in '{_options.RegistrationWaitTimeout}'.");
    }
}