using System.Buffers;

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

            var segments = pair.Key.Split(ConfigurationPath.KeyDelimiter,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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

            if (Children.Count == 0)
                return new ObjectNode(Name);

            if (IsSequentialArray())
            {
                var sortedChildren = Children
                    .OrderBy(c => int.Parse(c.Name!))
                    .Select(c => c.Convert())
                    .ToList();

                return new ArrayNode(Name, sortedChildren);
            }

            return new ObjectNode(Name, Children.Select(c => c.Convert()));
        }

        private bool IsSequentialArray()
        {
            var count = Children.Count;

            bool[]? arrayFromPool = null;
            var visited = count <= 256 ? stackalloc bool[count] : arrayFromPool = ArrayPool<bool>.Shared.Rent(count);

            visited[..count].Clear();

            try
            {
                foreach (var child in Children)
                {
                    if (!int.TryParse(child.Name, out var index) || index < 0 || index >= count)
                        return false;

                    if (visited[index])
                        return false;

                    visited[index] = true;
                }

                return true;
            }
            finally
            {
                if (arrayFromPool != null)
                {
                    ArrayPool<bool>.Shared.Return(arrayFromPool);
                }
            }
        }
    }
}