namespace Void.Proxy.Api.Network.Messages;

/// <summary>
/// Represents a disposable unit of data processed by a network channel.
/// </summary>
/// <remarks>
/// Consumers that retain ownership of a message must dispose it to release any pooled buffers or other resources held by the implementation.
/// </remarks>
public interface INetworkMessage : IDisposable;
