namespace Void.Proxy.Plugins.Common.Network.Protocol.Encryption;

public record EncryptionResponse(byte[] SharedSecret, byte[] VerifyToken, long Salt = 0);
