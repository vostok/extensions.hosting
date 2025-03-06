using JetBrains.Annotations;

using Microsoft.AspNetCore.Connections;

namespace Vostok.Extensions.AspNetCore.Builder;

/// <summary>
/// Represents configuration of <see cref="UnhandledExceptionMiddleware"/>.
/// </summary>
[PublicAPI]
public class UnhandledExceptionSettings
{
    /// <summary>
    /// Error response code to be used when an unhandled exception is observed.
    /// </summary>
    public int ErrorResponseCode { get; set; } = 500;

    /// <summary>
    /// List of exceptions to be ignored
    /// </summary>
    public List<Type> ExceptionsToIgnore = new() 
    {
        typeof(TaskCanceledException), 
        typeof(OperationCanceledException), 
        typeof(ConnectionResetException)
    };
}