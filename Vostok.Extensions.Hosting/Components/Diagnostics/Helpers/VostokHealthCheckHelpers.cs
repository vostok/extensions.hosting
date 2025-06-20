using Vostok.Hosting.Abstractions.Diagnostics;

using HealthStatus = Vostok.Hosting.Abstractions.Diagnostics.HealthStatus;
using IHealthCheck = Vostok.Hosting.Abstractions.Diagnostics.IHealthCheck;

namespace Vostok.Extensions.Hosting.Components.Diagnostics.Helpers;

using MicrosoftHealthCheck = Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck;
using MicrosoftHealthStatus = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus;
using MicrosoftHealthCheckResult = Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult;
using MicrosoftHealthCheckContext = Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext;
using MicrosoftHealthCheckRegistration = Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckRegistration;
using VostokHealthCheck = IHealthCheck;
using VostokHealthStatus = HealthStatus;

internal static class VostokHealthCheckHelpers
{
    /// <summary>
    /// Registers <see cref="MicrosoftHealthCheck"/> with <see cref="WellKnownHealthCheckNames.VostokHealthCheckTag"/> tag.
    /// </summary>
    public static MicrosoftHealthCheckRegistration RegisterWithVostokTag(
        string name,
        MicrosoftHealthCheck healthCheck) =>
        new(name, healthCheck, null, [WellKnownHealthCheckNames.VostokHealthCheckTag]);

    public static MicrosoftHealthCheck ToMicrosoftCheck(this VostokHealthCheck vostokCheck)
        => new VostokToMicrosoftAdapter(vostokCheck);

    public static VostokHealthStatus ToVostokStatus(this MicrosoftHealthStatus status) =>
        status switch
        {
            MicrosoftHealthStatus.Degraded => VostokHealthStatus.Degraded,
            MicrosoftHealthStatus.Healthy => VostokHealthStatus.Healthy,
            _ => VostokHealthStatus.Failing,
        };

    private class VostokToMicrosoftAdapter(VostokHealthCheck check) : MicrosoftHealthCheck
    {
        public async Task<MicrosoftHealthCheckResult> CheckHealthAsync(
            MicrosoftHealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var result = await check.CheckSafeAsync(cancellationToken).ConfigureAwait(false);

            return new MicrosoftHealthCheckResult(ToMicrosoftStatus(result.Status), result.Reason);
        }

        private static MicrosoftHealthStatus ToMicrosoftStatus(VostokHealthStatus status) =>
            status switch
            {
                VostokHealthStatus.Healthy => MicrosoftHealthStatus.Healthy,
                VostokHealthStatus.Degraded => MicrosoftHealthStatus.Degraded,
                _ => MicrosoftHealthStatus.Unhealthy
            };
    }
}