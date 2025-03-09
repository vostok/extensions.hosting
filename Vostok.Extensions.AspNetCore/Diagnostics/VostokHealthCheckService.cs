using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using Vostok.Extensions.AspNetCore.Helpers;
using Vostok.Hosting.Abstractions.Diagnostics;

using HealthReport = Microsoft.Extensions.Diagnostics.HealthChecks.HealthReport;

namespace Vostok.Extensions.AspNetCore.Diagnostics;

internal sealed class VostokHealthCheckService : HealthCheckService
{
    private readonly HealthCheckServiceOptions options;
    private readonly IServiceProvider serviceProvider;
    private readonly IHealthTracker vostokTracker;
    private readonly Type defaultServiceType;
    private readonly VostokDisposables disposables;

    public VostokHealthCheckService(
        IOptions<HealthCheckServiceOptions> options,
        IServiceProvider serviceProvider,
        IHealthTracker vostokTracker,
        Type defaultServiceType,
        VostokDisposables disposables)
    {
        this.defaultServiceType = defaultServiceType ?? throw new ArgumentNullException(nameof(defaultServiceType));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.vostokTracker = vostokTracker ?? throw new ArgumentNullException(nameof(vostokTracker));
        this.disposables = disposables ?? throw new ArgumentNullException(nameof(disposables));
        this.options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        RegisterMicrosoftChecksInVostokTracker();
    }

    public override Task<HealthReport> CheckHealthAsync(Func<HealthCheckRegistration, bool>? predicate, CancellationToken cancellationToken = new())
        => CreateService(CreateOptions()).CheckHealthAsync(predicate, cancellationToken);

    private void RegisterMicrosoftChecksInVostokTracker()
    {
        foreach (var registration in options.Registrations)
        {
            var vostokCheck = registration.Factory(serviceProvider).ToVostokCheck(registration);

            disposables.Add(vostokTracker.RegisterCheck(registration.Name, vostokCheck));
        }
    }

    private HealthCheckServiceOptions CreateOptions()
    {
        var result = new HealthCheckServiceOptions();

        foreach (var microsoftRegistration in options.Registrations)
            result.Registrations.Add(microsoftRegistration);

        foreach (var (name, check) in vostokTracker)
        {
            if (check.IsAdapter())
                continue;

            var registration = new HealthCheckRegistration(name, check.ToMicrosoftCheck(), null, ["vostok"]);

            result.Registrations.Add(registration);
        }

        return result;
    }

    private HealthCheckService CreateService(HealthCheckServiceOptions optionsToUse)
        => (HealthCheckService)ActivatorUtilities.CreateInstance(serviceProvider, defaultServiceType, Options.Create(optionsToUse));
}