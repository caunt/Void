namespace Void.Proxy.Plugins.Common.Services.Encryption;

public record EncryptionRequest(byte[] PublicKey, byte[] VerifyToken, string ServerId = "", bool ShouldAuthenticate = true);
