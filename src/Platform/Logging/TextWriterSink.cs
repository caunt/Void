namespace Void.Proxy.Logging;

using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

internal class TextWriterSink : ILogEventSink
{
    private readonly TextWriter _textWriter;
    private readonly MessageTemplateTextFormatter _formatter = new("[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj} {NewLine}{Exception}", null);

    public TextWriterSink(TextWriter textWriter)
    {
        _textWriter = textWriter;
    }

    public void Emit(LogEvent logEvent)
    {
        _formatter.Format(logEvent, _textWriter);
    }
}

