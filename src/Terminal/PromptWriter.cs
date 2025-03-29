using System.Text;

namespace Void.Terminal;

public class PromptWriter(PromptReader reader, StreamWriter writer) : TextWriter, IDisposable
{
    static PromptWriter()
    {
        if (WindowsConsole.TryEnableVirtualTerminalProcessing())
            return;

        throw new NotSupportedException("Virtual terminal processing is not supported.");
    }

    private PromptWriter() : this(null!, null!)
    {
        throw new NotSupportedException("This constructor is not supported.");
    }

    public override Encoding Encoding => writer.Encoding;

    public override void Write(string? value)
    {
        if (value is null)
            return;

        Write(value.ToUpper());
    }

    public override void WriteLine(string? value)
    {
        if (value is null)
            return;

        Write(value, true);
    }

    public void Write(string value, bool line = false)
    {
        var lines = GetBufferLines();

        Thread.Sleep(2000);
        SetCursorLeft();
        for (var i = 0; i < lines - 1; i++)
        {
            ClearCursorLine();
            SetCursorUp();
        }

        writer.Write(value);
        ClearCursorLine();

        if (line)
        {
            writer.WriteLine();
            writer.Write(reader.Prompt);
            writer.Write(reader.Buffer);
        }
    }

    public void UpdateBuffer(int length = 0)
    {
        var lines = GetBufferLines(length);

        SetCursorLeft();

        for (var i = 0; i < lines - 1; i++)
        {
            ClearCursorLine();
            SetCursorUp();
        }

        writer.Write(reader.Prompt);
        writer.Write(reader.Buffer);
        ClearCursorLine();
    }

    private int GetBufferLines(int length = 0)
    {
        // This width is not safe when the console is resized.
        var width = reader.Width;

        if (length is 0)
            length = reader.Buffer.Length + reader.Prompt.Length;

        if (width == length)
        {
            if (!OperatingSystem.IsWindows())
                return 1;

            if (Console.CursorLeft is 0)
                return 2;
        }

        var lines = Math.Max(1, (length + width - 1) / width);
        return lines;
    }

    private void SetCursorUp(byte value = 0)
    {
        WriteAnsiCommand('A', value);
    }

    private void SetCursorLeft(byte value = 0)
    {
        WriteAnsiCommand('G', value);
    }

    private void ClearCursorLine(byte value = 0)
    {
        WriteAnsiCommand('J', value);
    }

    private void WriteAnsiCommand(char command, params ReadOnlySpan<byte> parameters)
    {
        writer.Write('\x1B');
        writer.Write('[');

        if (parameters.Length > 0)
        {
            foreach (var parameter in parameters[..^1])
            {
                writer.Write(parameter);
                writer.Write(';');
            }

            writer.Write(parameters[^1]);
        }

        writer.Write(command);
    }
}