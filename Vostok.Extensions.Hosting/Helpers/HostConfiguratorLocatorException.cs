namespace Vostok.Extensions.Hosting.Helpers;

internal class HostConfiguratorLocatorException : Exception
{
    public HostConfiguratorLocatorException(string message)
        : base("The application to be launched could not be inferred from assembly attributes. " + message)
    {
    }
}