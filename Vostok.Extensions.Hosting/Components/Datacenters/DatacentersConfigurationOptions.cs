using System.Net;

using Vostok.Datacenters;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

public sealed class DatacentersConfigurationOptions
{
    private readonly List<Action<DatacentersSettings>> _customizations = [];

    public Func<IPAddress, string>? DatacenterMapping { get; set; }
    public Func<IReadOnlyCollection<string>>? ActiveDatacentersProvider { get; set; }

    public void AddCustomization(Action<DatacentersSettings> customization)
        => _customizations.Add(customization);

    internal void ApplyCustomizations(DatacentersSettings settings) =>
        _customizations.ForEach(customization => customization(settings));
}