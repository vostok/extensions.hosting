using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Vostok.ServiceDiscovery;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.Extensions.Hosting.Components.ServiceDiscovery;

internal sealed class ServiceBeaconHostedService(
    IServiceBeacon serviceBeacon,
    IOptions<ServiceBeaconConfigurationOptions> options,
    IHostApplicationLifetime applicationLifetime,
    ILogger<ServiceBeaconHostedService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        applicationLifetime.ApplicationStarted.Register(StartBeacon);
        applicationLifetime.ApplicationStopping.Register(StopBeacon);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    // Called after Kestrel started (if present).
    private void StartBeacon()
    {
        logger.LogInformation("Starting..");

        serviceBeacon.Start();
        WaitForServiceBeaconRegistration();
    }

    // Called before Kestrel stopped (if present).
    private void StopBeacon()
    {
        logger.LogInformation("Stopping..");

        serviceBeacon.Stop();
    }

    private void WaitForServiceBeaconRegistration()
    {
        var optionsValue = options.Value;

        if (!optionsValue.WaitRegistration)
            return;
        if (serviceBeacon is not ServiceBeacon actualBeacon)
            return;

        if (!actualBeacon.WaitForInitialRegistrationAsync().Wait(optionsValue.RegistrationWaitTimeout))
            throw new Exception($"Service beacon hasn't registered in '{optionsValue.RegistrationWaitTimeout}'.");
    }
}