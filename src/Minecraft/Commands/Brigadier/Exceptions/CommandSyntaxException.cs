using System;
using System.Text;

namespace Void.Minecraft.Commands.Brigadier.Exceptions;

public class CommandSyntaxException(ICommandExceptionType type, IMessage message, string input = "", int cursor = -1) : Exception(message.Value)
{
    public const int ContextAmount = 10;
    public static IBuiltInExceptionProvider BuiltInExceptions { get; set; } = new BuiltInExceptions();

    public override string Message => GetMessage();
    public ICommandExceptionType Type => type;
    public IMessage RawMessage => message;
    public string Input => input;
    public int Cursor => cursor;

    public string GetMessage()
    {
        var text = message.Value;
        var context = GetContext();

        if (context is not null)
            text += $" at position {cursor}: {context}";

        return text;
    }

    public string? GetContext()
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var builder = new StringBuilder();
        var cursorCorrected = Math.Min(input.Length, cursor);

        if (cursorCorrected > ContextAmount)
            builder.Append("...");

        builder.Append(input.AsSpan(Math.Max(0, cursorCorrected - ContextAmount), cursorCorrected));
        builder.Append("<--[HERE]");

        return builder.ToString();
    }
}
