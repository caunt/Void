namespace Void.Minecraft.Commands.Brigadier;

/// <summary>Exposes command input and a movable parsing cursor.</summary>
public interface IImmutableStringReader
{
    /// <summary>Gets the complete source string.</summary>
    public string Source { get; init; }
    /// <summary>Gets or sets the zero-based cursor.</summary>
    public int Cursor { get; set; }
    /// <summary>Gets the number of characters from the cursor through the end.</summary>
    public int RemainingLength { get; }
    /// <summary>Gets the total source length.</summary>
    public int TotalLength { get; }
    /// <summary>Gets the source prefix before the cursor.</summary>
    public string Read { get; }
    /// <summary>Gets the source suffix beginning at the cursor.</summary>
    public string Remaining { get; }
    /// <summary>Gets whether at least one character can be read.</summary>
    public bool CanRead { get; }
    /// <summary>Gets the character at the current cursor without advancing it.</summary>
    public char Peek { get; }

    /// <summary>
    /// Determines whether advancing the cursor by <paramref name="length"/> characters remains within the source.
    /// </summary>
    /// <param name="length">The number of characters to test from the current cursor position.</param>
    /// <returns>
    /// <see langword="true"/> when <paramref name="length"/> characters can be read without moving past <see cref="TotalLength"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool CanReadLength(int length);
    /// <summary>Gets a character at an offset from the cursor without advancing it.</summary>
    /// <param name="offset">The relative character offset.</param>
    /// <returns>The selected character.</returns>
    public char PeekAt(int offset);
}
