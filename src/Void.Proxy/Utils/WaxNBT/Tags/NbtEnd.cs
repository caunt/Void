namespace Void.Proxy.Utils.WaxNBT.Tags;

public class NbtEnd : NbtTag
{
    internal override void SerializeValue(ref NbtWriter writer) { }

    public static NbtEnd FromReader() => new();

    public override NbtTagType GetType() => NbtTagType.End;
}