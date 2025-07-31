using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Void.Tests.Streams;

public class CollectingTextWriter : TextWriter
{
    private readonly StringBuilder builder = new();
    private readonly object _lock = new();

    public override Encoding Encoding { get; } = Encoding.UTF8;

    public string Text
    {
        get
        {
            lock (_lock)
                return builder.ToString();
        }
    }

    public IEnumerable<string> Lines
    {
        get
        {
            var text = Text;
            return text.Split(["\r\n", "\n"], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public void Clear()
    {
        lock (_lock)
            builder.Clear();
    }

    public override void Write(char value)
    {
        lock (_lock)
            builder.Append(value);
    }

    public override void Write(string? value)
    {
        if (value != null)
        {
            lock (_lock)
                builder.Append(value);
        }
    }

    public override void Write(char[] buffer, int index, int count)
    {
        lock (_lock)
            builder.Append(buffer, index, count);
    }
}
