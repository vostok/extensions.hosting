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
            var vostokConfigurationProvider = new Vostok.Configuration.ConfigurationProvider();
            
            return vostokConfigurationProvider;
        });

        return builder;
    }

    
    class Node
    {
        public string? Name;
        public string? Value;
        public readonly List<Node> Children = new();

        public ISettingsNode Convert()
        {
            if (Value != null)
                return new ValueNode(Name, Value);

            if (Children.Count > 0)
            {
                if (int.TryParse(Children.First().Name, out _))
                {
                    return new ArrayNode(Name, Children.Select(c => c.Convert()).OrderBy(x =>
                    {
                        if (int.TryParse(x.Name, out var index))
                            return index;

                        return 0;
                    }).ToList());
                }
                return new ObjectNode(Name, Children.Select(c => c.Convert()));
            }
            
            return new ObjectNode(Name);
        }
    }
    
    private static ISettingsNode ConvertToISettingsNode(IConfiguration state)
    {
        var rootNode = new Node();
        
        foreach (var pair in state.AsEnumerable())
        {
            if (pair.Value == null)
                continue;
            
            var segments = pair.Key.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var root = rootNode;
            foreach (var segment in segments)
            {
                var existedChildren = root.Children.FirstOrDefault(c => c.Name == segment);
                if (existedChildren == null)
                {
                    var child = new Node() { Name = segment };
                    root.Children.Add(child);
                    root = child;
                }
                else
                {
                    root = existedChildren;
                }
            }
            root.Value = pair.Value;
        }

        var settingsNode = rootNode.Convert();
        return settingsNode;
    }   
}