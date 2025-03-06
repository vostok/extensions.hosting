using Vostok.Commons.Threading;

namespace Vostok.Extensions.AspNetCore.Helpers;

internal sealed class InitializedFlag() : AtomicBoolean(false);