using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Void.IntegrationTests.Infrastructure.IO;

public class CollectingTextWriter : TextWriter
{
    private readonly List<string> _lines = [];
    private readonly StringBuilder _currentLineBuilder = new();
    private readonly Lock _lock = new();

    public IReadOnlyList<string> Lines => [.. _lines];
    public string Text => string.Join('\n', Lines);
    public event Action<string>? OnLine;
    public override Encoding Encoding { get; } = Encoding.UTF8;

    public void Clear()
    {
        using var _ = _lock.EnterScope();

        _lines.Clear();
        _currentLineBuilder.Clear();
    }

    public override void Write(char value)
    {
        string[] completedLines;

        using (var _ = _lock.EnterScope())
            completedLines = AppendAndCollectCompletedLines([value]);

        foreach (var completedLine in completedLines)
            OnLine?.Invoke(completedLine);
    }

    public override void Write(string? value)
    {
        if (value is null)
            return;

        string[] completedLines;

        using (var _ = _lock.EnterScope())
            completedLines = AppendAndCollectCompletedLines(value);

        foreach (var completedLine in completedLines)
            OnLine?.Invoke(completedLine);
    }

    public override void Write(char[] buffer, int index, int count)
    {
        string[] completedLines;

        using (var _ = _lock.EnterScope())
            completedLines = AppendAndCollectCompletedLines(buffer.AsSpan(index, count));

        foreach (var completedLine in completedLines)
            OnLine?.Invoke(completedLine);
    }

    private string[] AppendAndCollectCompletedLines(ReadOnlySpan<char> value)
    {
        List<string>? completedLines = null;

        foreach (var character in value)
        {
            if (character is '\n')
            {
                var completedLine = _currentLineBuilder.ToString().Trim();
                _currentLineBuilder.Clear();

                if (completedLine.Length == 0)
                    continue;

                _lines.Add(completedLine);
                completedLines ??= [];
                completedLines.Add(completedLine);
                continue;
            }

            _currentLineBuilder.Append(character);
        }

        return completedLines?.ToArray() ?? [];
    }
}
