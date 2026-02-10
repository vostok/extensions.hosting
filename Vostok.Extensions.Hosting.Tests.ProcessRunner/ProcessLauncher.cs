using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

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

        return Launch(
            Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "Vostok.Extensions.Hosting.Tests.ProcessRunner.exe"),
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
            Verb = "runas",
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

        var output = new StringBuilder();
        var error = new StringBuilder();
        var outputComplete = new ManualResetEvent(false);
        var errorComplete = new ManualResetEvent(false);

        var process = new Process
        {
            StartInfo = processStartInfo,
            EnableRaisingEvents = true
        };
        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data == null)
                outputComplete.Set();
            else
                output.AppendLine(e.Data);
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data == null)
                errorComplete.Set();
            else
                error.AppendLine(e.Data);
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        var isExited = process.WaitForExit(timeoutMilliseconds);

        if (!isExited)
            process.Kill(entireProcessTree: true);

        // Ждем завершения чтения вывода, чтобы ничего не потерять (т.к. он асинхронный) 
        WaitHandle.WaitAll([outputComplete, errorComplete], 5000);

        return new ProcessResult
        {
            ExitCode = process.ExitCode,
            StandardOutput = output.ToString(),
            StandardError = error.ToString(),
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