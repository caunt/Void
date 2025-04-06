namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

public interface IPropertyValue
{
    public virtual TPropertyValue As<TPropertyValue>() where TPropertyValue : IPropertyValue
    {
        if (this is not TPropertyValue value)
            throw new InvalidCastException($"Property value {this} cannot be casted to {typeof(TPropertyValue)}");

        return value;
    }
}
