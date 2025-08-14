using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Void.Tests.Streams;

public class CollectingTextWriter : TextWriter
{
    private readonly StringBuilder _builder = new();
    private readonly Lock _lock = new();

    public override Encoding Encoding { get; } = Encoding.UTF8;

    public string Text
    {
        get
        {
            using var _ = _lock.EnterScope();
            return _builder.ToString();
        }
    }

    public IEnumerable<string> Lines
    {
        get
        {
            var text = Text;
            foreach (var line in text.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                yield return line.TrimEnd('\r');
        }
    }

    public void Clear()
    {
        using var _ = _lock.EnterScope();
        _builder.Clear();
    }

    public override void Write(char value)
    {
        using var _ = _lock.EnterScope();
        _builder.Append(value);
    }

    public override void Write(string? value)
    {
        if (value != null)
        {
            using var _ = _lock.EnterScope();
            _builder.Append(value);
        }
    }

    public override void Write(char[] buffer, int index, int count)
    {
        using var _ = _lock.EnterScope();
        _builder.Append(buffer, index, count);
    }
}
