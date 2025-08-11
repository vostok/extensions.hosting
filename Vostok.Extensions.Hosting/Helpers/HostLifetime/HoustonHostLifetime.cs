using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Vostok.Extensions.Hosting.Abstractions;

namespace Vostok.Extensions.Hosting.Helpers.HostLifetime;

internal sealed class HoustonHostLifetime(IHostLifetime baseHostLifetime, IServiceProvider serviceProvider)
    : IHostLifetime
{
    public async Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        await baseHostLifetime.WaitForStartAsync(cancellationToken);

        var hostConfigurator = serviceProvider.GetService<IVostokHostConfigurator>();

        //todo: может стоит переименовать IVostokHostConfigurator в IHoustonConfigurator?
        //Конфигуратор -- Хьюстон-специфичная штука как будто
        if (hostConfigurator == null)
            throw new InvalidOperationException("There is no registered Vostok host configurator");

        //todo: add ConfigureAfterBuild

        //todo: вынести отсюда восточные проверки
        RequirementsChecker.Check(hostConfigurator, serviceProvider);
        HoustonRequirementsChecker.Check(hostConfigurator, serviceProvider);
    }

    public Task StopAsync(CancellationToken cancellationToken) =>
        baseHostLifetime.StopAsync(cancellationToken);
}