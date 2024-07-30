using System.Buffers;

namespace Void.Proxy.API.Network.IO.Memory;

public record MemoryHolder : IDisposable
{
    private bool _disposed;
    public required IMemoryOwner<byte> Owner { get; init; }
    public required Memory<byte> Slice { get; set; }

    public void Dispose()
    {
        _disposed = true;
        Owner.Dispose();
    }

    public static MemoryHolder From(byte[] source)
    {
        var holder = RentExact(source.Length);
        source.CopyTo(holder.Slice);
        return holder;
    }

    public static MemoryHolder RentExact(int length)
    {
        var owner = MemoryPool<byte>.Shared.Rent(length);
        var slice = owner.Memory[..length];

        return new MemoryHolder
        {
            Owner = owner,
            Slice = slice
        };
    }

    public static MemoryHolder Concatenate(params MemoryHolder[] holders)
    {
        return Concatenate(holders.AsEnumerable());
    }

    public static MemoryHolder Concatenate(IEnumerable<MemoryHolder> holders)
    {
        var all = holders.ToArray();

        if (all.Length is 0)
            return RentExact(0);

        var length = all.Sum(holder => holder.Slice.Length);
        var merged = RentExact(length);

        var offset = 0;
        foreach (var holder in all)
        {
            holder.Slice.CopyTo(merged.Slice[offset..]);
            offset += holder.Slice.Length;
            holder.Dispose();
        }

        return merged;
    }

    ~MemoryHolder()
    {
        if (_disposed)
            return;

        // TODO uncomment Log.ForContext<MemoryHolder>().Warning("Found {TypeName} {Length} bytes long not disposed", nameof(MemoryHolder), Slice.Length);
        Dispose();
    }
}