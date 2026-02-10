using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Vostok.Extensions.Hosting.Tests.ProcessRunner;

public static class ProcessLauncher
{
    public static ProcessResult Launch<T>(
        Expression<Action<T>> method,
        string[]? args = null,
        int timeoutMilliseconds = 10000,
        Dictionary<string, string>? environmentVariables = null)
    {
        var type = typeof(T);
        var assemblyPath = type.Assembly.Location;
        var typeName = type.FullName;
        var methodName = ((MethodCallExpression)method.Body).Method.Name;

        var executableName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "Vostok.Extensions.Hosting.Tests.ProcessRunner.exe"
            : "Vostok.Extensions.Hosting.Tests.ProcessRunner";

        var executablePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            executableName);

        return Launch(
            executablePath,
            string.Concat(new[] {assemblyPath, typeName, methodName}.Concat(args ?? []).Select(x => $@"""{x}"" ")),
            timeoutMilliseconds,
            environmentVariables ?? new Dictionary<string, string>());
    }

    private static ProcessResult Launch(
        string fileName,
        string arguments,
        int timeoutMilliseconds,
        Dictionary<string, string> environmentVariables)
    {
        var processStartInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        foreach (var kv in environmentVariables)
        {
            processStartInfo.EnvironmentVariables[kv.Key] = kv.Value;
        }

        using var process = Process.Start(processStartInfo);

        if (process == null)
            throw new InvalidOperationException("Failed to start process");

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        var isExited = process.WaitForExit(timeoutMilliseconds);

        if (!isExited)
        {
            process.Kill(entireProcessTree: true);
            // В Linux kill асинхронный, нужно подождать перед получением ExitCode
            process.WaitForExit(5000);
        }

        // Ждем завершения чтения вывода, чтобы ничего не потерять (т.к. он асинхронный) 
        Task.WaitAll([outputTask, errorTask], 5000);

        return new ProcessResult
        {
            ExitCode = process.ExitCode,
            StandardOutput = outputTask.Result,
            StandardError = errorTask.Result,
            IsTimedOut = !isExited
        };
    }
    
    public class ProcessResult
    {
        public int ExitCode { get; init; }
        public string StandardOutput { get; init; } = "";
        public string StandardError { get; init; } = "";
        public bool IsTimedOut { get; init; }
    }
}