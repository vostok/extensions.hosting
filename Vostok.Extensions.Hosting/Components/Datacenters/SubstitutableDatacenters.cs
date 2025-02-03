using System.Net;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Vostok.Datacenters;

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
}

internal sealed class DatacenterSubstitutionMarker;

internal sealed class DatacenterSubstitutionValidator(
    IDatacenters substitutableDatacenters,
    [FromKeyedServices("_ActualDatacenters")] IDatacenters actualDatacenters) : IValidateOptions<DatacenterSubstitutionMarker>
{
    public ValidateOptionsResult Validate(string? _, DatacenterSubstitutionMarker __)
    {
        ((SubstitutableDatacenters)substitutableDatacenters).SubstituteWith(actualDatacenters);

        return ValidateOptionsResult.Skip;
    }
}