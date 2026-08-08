using System;
using System.Text;

namespace Void.Minecraft.Commands.Brigadier.Exceptions;

/// <summary>Represents a command parsing or execution syntax failure with optional input context.</summary>
/// <param name="type">The exception category.</param>
/// <param name="message">The unformatted error message.</param>
/// <param name="input">The command input, or an empty string when unavailable.</param>
/// <param name="cursor">The failure position, or <c>-1</c> when unavailable.</param>
public class CommandSyntaxException(ICommandExceptionType type, IMessage message, string input = "", int cursor = -1) : Exception(message.Value)
{
    /// <summary>Specifies the maximum number of input characters shown before the failure marker.</summary>
    public const int ContextAmount = 10;
    /// <summary>Gets or sets the global provider used by Brigadier parsing code.</summary>
    public static IBuiltInExceptionProvider BuiltInExceptions { get; set; } = new BuiltInExceptions();

    /// <summary>Gets the raw message augmented with cursor context when input is available.</summary>
    public override string Message => GetMessage();
    /// <summary>Gets the exception category.</summary>
    public ICommandExceptionType Type => type;
    /// <summary>Gets the unformatted message object.</summary>
    public IMessage RawMessage => message;
    /// <summary>Gets the captured command input.</summary>
    public string Input => input;
    /// <summary>Gets the captured cursor position.</summary>
    public int Cursor => cursor;

    /// <summary>Builds the displayed error message.</summary>
    /// <returns>The raw message, followed by a position and context excerpt when available.</returns>
    public string GetMessage()
    {
        var text = message.Value;
        var context = GetContext();

        if (context is not null)
            text += $" at position {cursor}: {context}";

        return text;
    }

    /// <summary>Builds the input excerpt ending at the failure marker.</summary>
    /// <returns>The excerpt, or <see langword="null"/> when the captured input is empty or whitespace.</returns>
    public string? GetContext()
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var builder = new StringBuilder();
        var cursorCorrected = Math.Min(input.Length, cursor);

        if (cursorCorrected > ContextAmount)
            builder.Append("...");

        var start = Math.Max(0, cursorCorrected - ContextAmount);
        builder.Append(input.AsSpan(start, cursorCorrected - start));
        builder.Append("<--[HERE]");

        return builder.ToString();
    }
}
