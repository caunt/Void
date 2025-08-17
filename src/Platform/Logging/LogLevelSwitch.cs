using Serilog.Core;
using Serilog.Events;
using Void.Proxy.Api.Logging;

namespace Void.Proxy.Logging;

public class LogLevelSwitch(LoggingLevelSwitch serilogSwitch) : ILogLevelSwitch
{
    public LogLevel Level
    {
        get => (LogLevel)serilogSwitch.MinimumLevel;
        set => serilogSwitch.MinimumLevel = (LogEventLevel)value;
    }
}
