using System.IO;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt
{
    public class NbtFile
    {
        private NbtWriter _writer = new NbtWriter();
        public NbtCompound Root;

        public NbtFile(string rootName = "")
        {
            Root = new NbtCompound(rootName);
        }

        public static NbtFile Parse(Stream stream, bool readName = true)
        {
            return Parse(new NbtReader(stream), readName);
        }

        public static NbtFile Parse(byte[] data, bool readName = true)
        {
            return Parse(new NbtReader(data), readName);
        }

        public static NbtFile Parse(NbtReader reader, bool readName = true)
        {
            return new NbtFile { Root = (NbtCompound)reader.ReadTag(readName: readName) };
        }

        public Stream Serialize()
        {
            Root.Serialize(ref _writer);

            var stream = _writer.GetStream();
            stream.Position = 0;

            return stream;
        }
    }
}