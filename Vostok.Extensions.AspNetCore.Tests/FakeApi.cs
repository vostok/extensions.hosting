using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Vostok.Extensions.Hosting;
using Vostok.Extensions.Hosting.Components.ApplicationIdentity;
using Vostok.Extensions.Hosting.Components.ApplicationLimits;
using Vostok.Extensions.Hosting.Components.Datacenters;
using Vostok.Extensions.Hosting.Components.Hercules;
using Vostok.Extensions.Hosting.Components.ServiceDiscovery;

namespace Vostok.Extensions.AspNetCore.Tests;

public class FakeApi(WebApplication application) : IAsyncDisposable, IDisposable
{
    private readonly WebApplication application = application;

    public HttpClient Client { get; } = new()
    {
        BaseAddress = new UriBuilder
        {
            Scheme = "http",
            Port = GetApplicationPort(application),
            Host = "127.0.0.1"
        }.Uri
    };

    private static int GetApplicationPort(WebApplication application) =>
        new Uri(application.Urls.First()).Port;

    public Task StopAsync(CancellationToken cancellationToken = default) => application.StopAsync(cancellationToken);

    public static async Task<FakeApi> StartAsync(
        Action<IVostokBuilder>? configureVostok = null,
        Action<WebApplicationBuilder>? configureBuilder = null,
        Action<WebApplication>? configureApp = null
    )
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://*:0"); // any address, any port
        builder.Services.AddOpenTelemetry().WithTracing();
        configureBuilder?.Invoke(builder);

        var vostokBuilder = builder.UseVostok();
        vostokBuilder
            .WithApplicationLimits()
            .WithWebComponents()
            .WithDatacenters()
            .DisableHerculesSink()
            .DisableServiceBeacon()
            .WithApplicationIdentity(opts =>
            {
                opts.Project = "Vostok.Extensions";
                opts.Application = "FakeApi";
                opts.Environment = "test";
                opts.Instance = "0";
            });

        configureVostok?.Invoke(vostokBuilder);

        var app = builder.Build();
        app.UseVostokMiddlewares();
        configureApp?.Invoke(app);

        await app.StartAsync();
        return new FakeApi(app);
    }

    public void Dispose()
    {
        ((IDisposable)application).Dispose();
        ((IDisposable)application).Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await application.DisposeAsync();
        await application.DisposeAsync();
    }
}