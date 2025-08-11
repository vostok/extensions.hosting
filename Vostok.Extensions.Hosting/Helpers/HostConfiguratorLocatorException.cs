namespace Vostok.Extensions.Hosting.Helpers;

internal class HostConfiguratorLocatorException : Exception
{
    public HostConfiguratorLocatorException(string message)
        : base("The host configurator to be used could not be inferred from assembly attributes. " + message)
    {
    }
}