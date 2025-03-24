using Microsoft.Extensions.Options;

using Vostok.Datacenters;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

internal sealed class DatacentersSettingsFactory(
    IEnumerable<IConfigureOptions<DatacentersSettings>> setups,
    IEnumerable<IPostConfigureOptions<DatacentersSettings>> postConfigures,
    IEnumerable<IValidateOptions<DatacentersSettings>> validations,
    IOptions<DatacentersOptions> optionsMonitor)
    : OptionsFactory<DatacentersSettings>(setups, postConfigures, validations)
{
    protected override DatacentersSettings CreateInstance(string name) =>
        new(optionsMonitor.Value.DatacenterMapping,
            optionsMonitor.Value.ActiveDatacentersProvider);
}