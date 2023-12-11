using MinecraftProxy.Utils.WaxNBT.Tags;

namespace MinecraftProxy.Utils.WaxNBT;

public class NbtFile
{
    private NbtWriter _writer = new();
    public NbtCompound Root;

    public NbtFile(string rootName = "") => Root = new NbtCompound(rootName);

    public static NbtFile Parse(Stream stream) => Parse(new NbtReader(stream));

    public static NbtFile Parse(byte[] data) => Parse(new NbtReader(data));

    public static NbtFile Parse(NbtReader reader) => new() { Root = (NbtCompound)reader.ReadTag() };

    public Stream Serialize()
    {
        Root.Serialize(ref _writer);

        Stream stream = _writer.GetStream();
        stream.Position = 0;

        return stream;
    }
}