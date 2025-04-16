using JetBrains.Annotations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Primitives;

using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources;
using Vostok.Configuration.Sources.Manual;

using IConfigurationSource = Vostok.Configuration.Abstractions.IConfigurationSource;

namespace Vostok.Extensions.Hosting.Builder;

[PublicAPI]
public static class VostokBuilderConfigurationExtensions
{
    public static IVostokBuilder WithVostokConfigurationProvider(this IVostokBuilder builder)
    {
        builder.Services.TryAddSingleton<IConfigurationSource>(provider =>
        {
            var rootConfiguration = provider.GetService<IConfiguration>()!;

            var mySource = new ManualFeedSource();
            mySource.Push(ConvertToISettingsNode(rootConfiguration));
            ChangeToken.OnChange(() => rootConfiguration.GetReloadToken(),
                state => mySource.Push(ConvertToISettingsNode(state)),
                rootConfiguration);
            return mySource;
        });
        builder.Services.TryAddSingleton<Vostok.Configuration.Abstractions.IConfigurationProvider>(provider =>
        {
            var megaSource = provider.GetService<IConfigurationSource>();
            var vostokConfigurationProvider = new Vostok.Configuration.ConfigurationProvider();
            
            //(deniaa) Можно будет добавить экстеншнов, которые этот сорс будут устанавливать в провайдер для указанного T.
            vostokConfigurationProvider.TrySetupSourceFor<string>(megaSource.ScopeTo("some/nested/thing"));
            
            return vostokConfigurationProvider;
        });

        return builder;
    }

    private static ISettingsNode ConvertToISettingsNode(IConfiguration state)
    {
        foreach (var keyValuePair in state.AsEnumerable())
        {
            Console.WriteLine(keyValuePair.Key + " " + keyValuePair.Value);
        }

        return new ValueNode("foo");
    }   
}