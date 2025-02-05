using System.Net;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;
using Vostok.Extensions.Hosting.Builder;

namespace Vostok.Extensions.Hosting.Components.Datacenters;

internal sealed class SubstitutableDatacenters : IDatacenters
{
    private IDatacenters _baseDatacenters = new EmptyDatacenters();

    public string? GetLocalDatacenter() =>
        _baseDatacenters.GetLocalDatacenter();

    public string? GetDatacenter(IPAddress address) =>
        _baseDatacenters.GetDatacenter(address);

    public string? GetDatacenter(string hostname) =>
        _baseDatacenters.GetDatacenter(hostname);

    public string? GetDatacenterWeak(string hostname) =>
        _baseDatacenters.GetDatacenterWeak(hostname);

    public IReadOnlyCollection<string> GetActiveDatacenters() =>
        _baseDatacenters.GetActiveDatacenters();

    public void SubstituteWith(IDatacenters newDatacenters) =>
        _baseDatacenters = newDatacenters;

    public IDatacenters GetBase() =>
        _baseDatacenters;
    
    public sealed class SubstitutionMarker;
}

internal sealed class DatacenterSubstitutionValidator(IServiceProvider provider)
    : IValidateOptions<SubstitutableDatacenters.SubstitutionMarker>
{
    public ValidateOptionsResult Validate(string? _, SubstitutableDatacenters.SubstitutionMarker __)
    {
        var substitutableDatacenters = provider.GetService<IDatacenters>() as SubstitutableDatacenters;
        var actualDatacenters = provider.GetKeyedService<IDatacenters>(DatacentersBuilderExtensions.ActualDatacentersKey);

        if (substitutableDatacenters is not null && actualDatacenters is not null)
            substitutableDatacenters.SubstituteWith(actualDatacenters);

        return ValidateOptionsResult.Skip;
    }
}