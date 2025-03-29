using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Spectre;

namespace Void.Proxy.Console;

public class ConsoleSink(ConsoleService console, string outputTemplate, bool renderTextAsMarkup = false) : ILogEventSink
{
    private readonly SpectreConsoleSink _spectreSink = new(outputTemplate, renderTextAsMarkup);

    public void Emit(LogEvent logEvent)
    {
        // console.PrepareRender();
        _spectreSink.Emit(logEvent);
        // console.Render(true);
    }
}