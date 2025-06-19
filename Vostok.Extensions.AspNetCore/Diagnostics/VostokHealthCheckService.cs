using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hosting.Abstractions.Diagnostics;

using HealthReport = Microsoft.Extensions.Diagnostics.HealthChecks.HealthReport;

namespace Vostok.Extensions.AspNetCore.Diagnostics;

internal sealed class VostokHealthCheckService : HealthCheckService
{
    private readonly HealthCheckServiceOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHealthTracker _vostokTracker;
    private readonly Type _defaultServiceType;
    private readonly VostokDisposables _disposables;

    public VostokHealthCheckService(
        IOptions<HealthCheckServiceOptions> options,
        IServiceProvider serviceProvider,
        IHealthTracker vostokTracker,
        Type defaultServiceType,
        VostokDisposables disposables)
    {
        _defaultServiceType = defaultServiceType ?? throw new ArgumentNullException(nameof(defaultServiceType));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _vostokTracker = vostokTracker ?? throw new ArgumentNullException(nameof(vostokTracker));
        _disposables = disposables ?? throw new ArgumentNullException(nameof(disposables));
        _options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        RegisterMicrosoftChecksInVostokTracker();
    }

    public override Task<HealthReport> CheckHealthAsync(Func<HealthCheckRegistration, bool>? predicate, CancellationToken cancellationToken = new())
        => CreateService(CreateOptions()).CheckHealthAsync(predicate, cancellationToken);

    private void RegisterMicrosoftChecksInVostokTracker()
    {
        foreach (var registration in _options.Registrations)
        {
            var vostokCheck = registration.Factory(_serviceProvider).ToVostokCheck(registration);

            _disposables.Add(_vostokTracker.RegisterCheck(registration.Name, vostokCheck));
        }
    }

    private HealthCheckServiceOptions CreateOptions()
    {
        var result = new HealthCheckServiceOptions();

        foreach (var microsoftRegistration in _options.Registrations)
            result.Registrations.Add(microsoftRegistration);

        foreach (var (name, check) in _vostokTracker)
        {
            if (check.IsAdapter())
                continue;

            var registration = new HealthCheckRegistration(name, check.ToMicrosoftCheck(), null, ["vostok"]);

            result.Registrations.Add(registration);
        }

        return result;
    }

    private HealthCheckService CreateService(HealthCheckServiceOptions optionsToUse)
        => (HealthCheckService)ActivatorUtilities.CreateInstance(_serviceProvider, _defaultServiceType, Options.Create(optionsToUse));
}