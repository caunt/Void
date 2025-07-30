using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Void.Tests.Streams;

public class CollectingTextWriter : TextWriter
{
    private readonly StringBuilder builder = new();

    public override Encoding Encoding { get; } = Encoding.UTF8;

    public string Text => builder.ToString();

    public IEnumerable<string> Lines => Text.Split(["\r\n", "\n"], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    public void Clear()
    {
        builder.Clear();
    }

    public override void Write(char value)
    {
        builder.Append(value);
    }

    public override void Write(string? value)
    {
        if (value != null)
        {
            builder.Append(value);
        }
    }

    public override void Write(char[] buffer, int index, int count)
    {
        builder.Append(buffer, index, count);
    }
}
