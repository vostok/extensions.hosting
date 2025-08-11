using System.Text;

using JetBrains.Annotations;

namespace Vostok.Extensions.Hosting.Helpers;

[PublicAPI]
public class RequirementsCheckException : Exception
{
    public RequirementsCheckException(Type configuratorType, IReadOnlyList<string> errors)
        : base(FormatMessage(configuratorType, errors))
    {
    }

    private static string FormatMessage(Type configuratorType, IReadOnlyList<string> errors)
    {
        var builder = new StringBuilder();

        builder.Append(
            $"Configured hosting environment has failed to satisfy some of the configurator's ('{configuratorType.Name}') requirements:");

        foreach (var error in errors)
        {
            builder
                .AppendLine()
                .Append('\t')
                .Append(error);
        }

        return builder.ToString();
    }
}