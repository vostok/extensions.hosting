using Vostok.Logging.Abstractions;

namespace Vostok.Extensions.Hosting.Components.Logging.Builder;

internal delegate ILog LogCustomization(ILog log, IServiceProvider serviceProvider);