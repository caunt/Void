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

    public bool CanReadLength(int length);
    public char PeekAt(int offset);
}
