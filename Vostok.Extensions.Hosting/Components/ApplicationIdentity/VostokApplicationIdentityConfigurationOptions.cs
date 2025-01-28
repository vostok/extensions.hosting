namespace Vostok.Extensions.Hosting.Components.ApplicationIdentity;

public sealed class VostokApplicationIdentityConfigurationOptions
{
    public string Project { get; set; }
    public string? Subproject { get; set; }
    public string Environment { get; set; }
    public string Application { get; set; }
    public string Instance { get; set; }
}