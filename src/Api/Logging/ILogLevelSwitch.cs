using Microsoft.Extensions.Logging;

namespace Void.Proxy.Api.Logging;

public interface ILogLevelSwitch
{
    public LogLevel Level { get; set; }
}
