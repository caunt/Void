using System.Diagnostics.CodeAnalysis;
using NuGet.Common;

namespace Void.Proxy.Plugins.Dependencies.Nuget;

[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Why would I remove suppress message if I need it?")]
[SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "Do not template NuGet messages")]
public class NuGetLogger(Microsoft.Extensions.Logging.ILogger logger) : NuGet.Common.ILogger
{
    public void Log(NuGet.Common.LogLevel level, string data)
    {
        logger.Log((Microsoft.Extensions.Logging.LogLevel)level, data);
    }

    public void Log(ILogMessage message)
    {
        logger.Log((Microsoft.Extensions.Logging.LogLevel)message.Level, message.Message);
    }

    public Task LogAsync(NuGet.Common.LogLevel level, string data)
    {
        logger.Log((Microsoft.Extensions.Logging.LogLevel)level, data);
        return Task.CompletedTask;
    }

    public Task LogAsync(ILogMessage message)
    {
        logger.Log((Microsoft.Extensions.Logging.LogLevel)message.Level, message.Message);
        return Task.CompletedTask;
    }

    public void LogDebug(string data)
    {
        logger.LogDebug(data);
    }

    public void LogError(string data)
    {
        logger.LogError(data);
    }

    public void LogInformation(string data)
    {
        logger.LogInformation(data);
    }

    public void LogInformationSummary(string data)
    {
        logger.LogInformation(data);
    }

    public void LogMinimal(string data)
    {
        logger.LogInformation(data);
    }

    public void LogVerbose(string data)
    {
        logger.LogTrace(data);
    }

    public void LogWarning(string data)
    {
        logger.LogWarning(data);
    }
}
