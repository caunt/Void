namespace Void.Minecraft.Commands.Brigadier;

public interface IImmutableStringReader
{
    public string Source { get; init; }
    public int Cursor { get; set; }
    public int RemainingLength { get; }
    public int TotalLength { get; }
    public string Read { get; }
    public string Remaining { get; }
    public bool CanRead { get; }
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
    public char PeekAt(int offset);
}
