using Microsoft.Extensions.Configuration;

using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Extensions.Hosting.Components.Configuration;

internal static class ConfigurationConverter
{
    public static ISettingsNode ConvertToSettingsNode(IConfiguration state)
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
                var existedChildren = root.Children.FirstOrDefault(c => string.Equals(c.Name, segment, StringComparison.OrdinalIgnoreCase));
                if (existedChildren == null)
                {
                    var child = new Node { Name = segment };
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
    
    private sealed class Node
    {
        public string? Name;
        public string? Value;
        public readonly List<Node> Children = [];

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
}