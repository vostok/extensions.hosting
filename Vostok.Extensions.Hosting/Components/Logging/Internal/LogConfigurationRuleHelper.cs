using Vostok.Logging.Configuration;

namespace Vostok.Extensions.Hosting.Components.Logging.Internal;

internal static class LogConfigurationRuleHelper
{
    public static LogConfigurationRule WithLog(this LogConfigurationRule rule, string logName)
    {
        if (rule.Log != logName && rule.Log != null)
            throw new ArgumentException($"Rule for {rule.Log} log can't be used for {logName} log.");

        rule = new LogConfigurationRule
        {
            Enabled = rule.Enabled,
            Log = logName,
            Operation = rule.Operation,
            Properties = rule.Properties,
            Source = rule.Source,
            MinimumLevel = rule.MinimumLevel
        };

        return rule;
    }
}