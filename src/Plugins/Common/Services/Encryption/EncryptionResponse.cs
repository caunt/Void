namespace Void.Proxy.Plugins.Common.Services.Encryption;

public record EncryptionResponse(byte[] SharedSecret, byte[] VerifyToken, long Salt = 0);
