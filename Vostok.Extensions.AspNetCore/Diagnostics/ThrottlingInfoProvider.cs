using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Throttling;

namespace Vostok.Extensions.AspNetCore.Diagnostics;

public class ThrottlingInfoProvider(ThrottlingProvider throttlingProvider) : IDiagnosticInfoProvider
{
    public object Query() => throttlingProvider.CurrentInfo;
}