using System.Collections;
using System.Net;
using System.Runtime;
using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Vostok.Clusterclient.Core;
using Vostok.ClusterConfig.Client;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Commons.Environment;
using Vostok.Commons.Threading;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Primitives;
using Vostok.Context;
using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Components.Logging.Substitution;
using Vostok.Extensions.Hosting.Components.ThreadPool;
using Vostok.Extensions.Hosting.Configuration;
using Vostok.Extensions.Hosting.Helpers;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.Metrics;
using Vostok.ServiceDiscovery.Abstractions;
using Vostok.Tracing.Abstractions;
using Vostok.ZooKeeper.Client;
using Vostok.ZooKeeper.Client.Abstractions;

namespace Vostok.Extensions.Hosting;

internal sealed class VostokWarmup(
    IServiceProvider provider,
    IEnumerable<VostokWarmup.CustomWarmupAction> customStartupActions,
    ILogger<VostokWarmup> logger)
{
    public delegate void CustomWarmupAction(IServiceProvider provider);

    public async Task WarmupVostokAsync()
    {
        // note (ponomaryovigor, 01.04.2025): Order is important to prevent cyclic dependencies.
        ConfigureThreadPool();
        WarmupConfiguration();
        await WarmupClusterConfigClientAsync();
        WarmupVostokLog();
        WarmupDatacenters();
        WarmupVostokTracer();
        WarmupVostokMetricsContext();
        WarmupHerculesSink();
        await WarmupServiceDiscoveryAsync();
        ConfigureFlowingContext();

        foreach (var startupAction in customStartupActions)
            startupAction(provider);

        LogEnvironmentInfo();
        LogDotnetEnvironmentVariables();
        LogApplicationIdentity();
        LogLocalDatacenter();
        LogApplicationLimits();
        LogApplicationReplication();
        LogThreadPoolSettings();
    }

    private void ConfigureThreadPool()
    {
        var threadPoolOptionsMonitor = provider.GetRequiredService<IOptionsMonitor<ThreadPoolOptions>>();
        var applicationLimits = provider.GetService<IVostokApplicationLimits>();

        Configure(threadPoolOptionsMonitor.CurrentValue);

        var subscription = threadPoolOptionsMonitor.OnChange(Configure);
        if (subscription is not null)
            provider.RegisterDisposable(subscription);

        return;

        void Configure(ThreadPoolOptions options)
        {
            if (!options.ConfigureThreadPool)
                return;

            ThreadPoolHelper.Setup(options.ThreadPoolTuningMultiplier, applicationLimits?.CpuUnits);
        }
    }

    private void WarmupConfiguration()
    {
        var configurationProvider = provider.GetService<IConfigurationProvider>();

        if (configurationProvider is ConfigurationProvider actualProvider)
            ConfigurationProvider.TrySetDefault(actualProvider, true);
    }

    private void WarmupVostokLog()
    {
        var vostokLog = provider.GetRequiredService<ILog>();
        LogProvider.Configure(vostokLog, true);

        var initializationLog = provider.GetInitializationLog() as SubstitutableLog;
        initializationLog?.SubstituteWith(vostokLog);

        // note: We don't need static sharing after Vostok warmup.
        InitializationLog.Clear();
    }

    private void WarmupVostokTracer()
    {
        var vostokTracer = provider.GetRequiredService<ITracer>();
        TracerProvider.Configure(vostokTracer, true);
    }

    private void WarmupVostokMetricsContext()
    {
        var vostokMetrics = provider.GetService<IVostokApplicationMetrics>();
        if (vostokMetrics is not null)
            MetricContextProvider.Configure(vostokMetrics.Root, true);
    }

    private void WarmupHerculesSink()
    {
        var herculesSink = provider.GetService<IHerculesSink>();
        if (herculesSink is not null)
            HerculesSinkProvider.Configure(herculesSink, true);
    }

    private void WarmupDatacenters()
    {
        var datacenters = provider.GetService<IDatacenters>();
        if (datacenters is null)
            return;

        // note (ponomaryovigor, 01.04.2025): Warmup providers to prevent potential cyclic dependencies with CCC.
        _ = datacenters.GetActiveDatacenters();
        _ = datacenters.GetDatacenter(IPAddress.None);
        DatacentersProvider.Configure(datacenters, true);
    }

    private async Task WarmupServiceDiscoveryAsync()
    {
        var zooKeeperClient = provider.GetService<IZooKeeperClient>();
        if (zooKeeperClient is ZooKeeperClient actualClient)
        {
            logger.LogInformation("Warming up ZooKeeper connection..");
            await actualClient.ExistsAsync("/");
        }

        _ = provider.GetService<IServiceBeacon>();

        var serviceNameProvider = provider.GetRequiredService<ServiceNameProvider>();
        ClusterClientDefaults.ClientApplicationName = serviceNameProvider.ServiceName;
    }

    private async Task WarmupClusterConfigClientAsync()
    {
        var clusterConfigClient = provider.GetService<IClusterConfigClient>();
        if (clusterConfigClient is null)
            return;

        await clusterConfigClient.GetAsync(Guid.NewGuid().ToString());
        ClusterConfigDeferredConfigurationSource.ActivateAllProviders(clusterConfigClient);

        if (clusterConfigClient is ClusterConfigClient actualClient)
            ClusterConfigClient.TrySetDefaultClient(actualClient);
    }

    private void ConfigureFlowingContext()
    {
        var flowingContextLogger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("FlowingContext");
        FlowingContext.Configuration.ErrorCallback = (errorMessage, error) => flowingContextLogger.LogError(error, errorMessage);
    }

    private void LogEnvironmentInfo()
    {
        logger.LogInformation("Application user = '{User}'.", Environment.UserName);
        logger.LogInformation("Application host = '{Host}'.", EnvironmentInfo.Host);
        logger.LogInformation("Application host FQDN = '{HostFQDN}'.", EnvironmentInfo.FQDN);
        logger.LogInformation("Application process id = '{ProcessId}'.", EnvironmentInfo.ProcessId);
        logger.LogInformation("Application process name = '{ProcessName}'.", EnvironmentInfo.ProcessName);
        logger.LogInformation("Application base directory = '{BaseDirectory}'.", EnvironmentInfo.BaseDirectory);
        logger.LogInformation("Application current directory = '{CurrentDirectory}'.", Environment.CurrentDirectory);
        logger.LogInformation("Application OS = '{OperatingSystem}'.", RuntimeInformation.OSDescription);
        logger.LogInformation("Application bitness = '{Bitness}'.", Environment.Is64BitProcess ? "x64" : "x86");
        logger.LogInformation("Application framework = '{Framework}'.", RuntimeInformation.FrameworkDescription);
        logger.LogInformation("Application GC type = '{GCType}'.", GCSettings.IsServerGC ? "Server" : "Workstation");
    }

    private void LogDotnetEnvironmentVariables()
    {
        try
        {
            var dotnetPrefixes = new[] { "DOTNET_", "COMPlus_", "ASPNETCORE_" };
            var dotnetVariables = new Dictionary<string, string>();

            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                if (entry is { Key: string stringKey, Value: string stringValue } &&
                    dotnetPrefixes.Any(p => stringKey.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)))
                    dotnetVariables[stringKey] = stringValue;
            }

            logger.LogInformation("Application dotnet environment variables = '{EnvironmentVariables}'.", dotnetVariables);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private void LogApplicationIdentity()
    {
        var identity = provider.GetService<IVostokApplicationIdentity>();
        if (identity is null)
            return;

        logger.LogInformation(
            "Application identity: project: '{Project}', subproject: '{Subproject}', environment: '{Environment}', application: '{Application}', instance: '{Instance}'.",
            identity.Project, identity.Subproject ?? "N/A", identity.Environment, identity.Application, identity.Instance);
    }

    private void LogLocalDatacenter()
    {
        var datacenters = provider.GetService<IDatacenters>();
        if (datacenters is null)
            return;

        logger.LogInformation("Application datacenter: {DatacenterName}.", datacenters.GetLocalDatacenter() ?? "unknown");
    }

    private void LogApplicationLimits()
    {
        var limits = provider.GetService<IVostokApplicationLimits>();
        if (limits is null)
            return;

        logger.LogInformation(
            "Application limits: {CpuLimit} CPU, {MemoryLimit} memory.",
            limits.CpuUnits?.ToString("F2") ?? "unlimited",
            limits.MemoryBytes.HasValue ? new DataSize(limits.MemoryBytes.Value).ToString() : "unlimited");
    }

    private void LogApplicationReplication()
    {
        var replication = provider.GetService<IVostokApplicationReplicationInfo>();
        if (replication is null)
            return;

        logger.LogInformation("Application replication: instance {InstanceIndex} of {InstanceCount}.",
            replication.InstanceIndex, replication.InstancesCount);
    }

    private void LogThreadPoolSettings()
    {
        var state = ThreadPoolUtility.GetPoolState();
        logger.LogInformation("Thread pool configuration: {MinWorkerThreads} min workers, {MinIOCPThreads} min IOCP.",
            state.MinWorkerThreads, state.MinIocpThreads);
    }
}