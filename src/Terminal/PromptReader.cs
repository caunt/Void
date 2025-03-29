using System.Text;

namespace Void.Terminal;

public class PromptReader : IDisposable
{
    private readonly PromptWriter _writer;

    public string Prompt { get; set; }
    public StringBuilder Buffer { get; set; }

    public TextWriter TextWriter => TextWriter.Synchronized(_writer);
    public int Width => Console.WindowWidth;

    public PromptReader(string prompt = "> ", StringBuilder? buffer = null, Stream? input = null, Stream? output = null)
    {
        if (input is not null)
            throw new NotSupportedException("Custom input streams are not supported yet.");

        var stdout = output ?? Console.OpenStandardOutput();
        var stdoutWriter = new StreamWriter(stream: stdout, encoding: Console.OutputEncoding, bufferSize: 256, leaveOpen: true)
        {
            AutoFlush = true
        };

        _writer = new PromptWriter(this, stdoutWriter);

        Buffer = buffer ?? new StringBuilder();
        Prompt = prompt;
    }

    public async ValueTask<string> ReadLineAsync()
    {
        await Task.Yield();

        do
        {
            var info = Console.ReadKey(true);

            if (info.Key is ConsoleKey.Enter)
                break;

            var length = Buffer.Length + Prompt.Length;

            if (info.Key is ConsoleKey.Backspace)
            {
                if (Buffer.Length > 0)
                    Buffer.Remove(Buffer.Length - 1, 1);

                _writer.UpdateBuffer(length);
            }
            else
            {
                Buffer.Append(info.KeyChar);
                _writer.UpdateBuffer(length);
            }
        }
        while (true);

        return Buffer.ToString();
    }

    public void Dispose()
    {
        _writer.Dispose();
        Buffer.Clear();

        GC.SuppressFinalize(this);
    }
}
