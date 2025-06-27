using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.Internal;

internal delegate ILog LogCustomization(ILog log, IServiceProvider serviceProvider);