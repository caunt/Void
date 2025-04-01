using System.Text;

namespace Void.Terminal;

public class PromptReader : IDisposable
{
    private readonly PromptWriter _writer;

    public string Prompt { get; set; }
    public StringBuilder Buffer { get; set; }

    public TextWriter TextWriter => TextWriter.Synchronized(_writer);
    public int Width { get; } = Console.WindowWidth;

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

    public void HideCursor()
    {
        _writer.HideCursor();
    }

    public void ShowCursor()
    {
        _writer.ShowCursor();
    }

    public void ResetStyle()
    {
        _writer.ResetStyle();
    }

    public async ValueTask<string> ReadLineAsync(Autocompletion? autocompletion = null, CancellationToken cancellationToken = default)
    {
        await Task.Yield();

        var suggestionsPrefix = string.Empty;

        do
        {
            if (!Console.KeyAvailable)
            {
                await Task.Delay(50, cancellationToken);
                continue;
            }

            var info = Console.ReadKey(true);

            if (info.Key is ConsoleKey.Enter)
            {
                try
                {
                    return Buffer.ToString();
                }
                finally
                {
                    Buffer.Clear();
                }
            }

            var length = Buffer.Length + Prompt.Length;

            if (info.Key is ConsoleKey.Backspace)
            {
                if (Buffer.Length > 0)
                    Buffer.Remove(Buffer.Length - 1, 1);

                _writer.UpdateBuffer(length);
            }
            else if (info.Key is ConsoleKey.Tab)
            {
                if (autocompletion is null)
                    continue;

                var input = Buffer.ToString();
                var words = input.Split(' ');
                var currentWord = words[^1];

                if (!input.StartsWith(suggestionsPrefix) || input.EndsWith(' '))
                    suggestionsPrefix = input;

                var suggestions = await autocompletion(suggestionsPrefix, cancellationToken);
                var matches = suggestions.ToArray();

                if (matches.Length is 0)
                {
                    suggestionsPrefix = string.Empty;
                    continue;
                }

                var nextSuggestion = matches[(matches.IndexOf(currentWord) + 1) % matches.Length];

                Buffer.Remove(Buffer.Length - currentWord.Length, currentWord.Length);
                Buffer.Append(nextSuggestion);

                _writer.UpdateBuffer(length);
            }
            else
            {
                Buffer.Append(info.KeyChar);
                _writer.UpdateBuffer(length);
            }
        }
        while (!cancellationToken.IsCancellationRequested);

        throw new OperationCanceledException();
    }

    public void Dispose()
    {
        _writer.Dispose();
        Buffer.Clear();

        GC.SuppressFinalize(this);
    }
}
