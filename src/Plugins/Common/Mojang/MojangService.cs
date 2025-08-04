using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Void.Minecraft.Mojang;
using Void.Minecraft.Players.Extensions;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Crypto;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Settings;
using Void.Proxy.Plugins.Common.Crypto;

namespace Void.Proxy.Plugins.Common.Mojang;

public class MojangService(ICryptoService crypto, ISettings settings) : IMojangService
{
    private static readonly HttpClient Client = new();
    private static readonly string SessionServer = Environment.GetEnvironmentVariable("VOID_MOJANG_SESSIONSERVER") ?? "https://sessionserver.mojang.com/session/minecraft/hasJoined";
    private static readonly bool PreventProxyConnections = bool.TryParse(Environment.GetEnvironmentVariable("VOID_MOJANG_PREVENT_PROXY_CONNECTIONS"), out var value) && value;

    public async ValueTask<GameProfile?> VerifyAsync(IPlayer player, CancellationToken cancellationToken = default)
    {
        if (player.Profile is not { } profile)
            throw new ArgumentNullException(nameof(player), "Player profile should be set in order to verify his session");

        if (settings.Offline)
            return new GameProfile(profile.Username, Uuid.Offline(profile.Username));

        var sharedSecret = player.Context.Services.GetRequiredService<ITokenHolder>().Get(TokenType.SharedSecret);
        var publicKey = crypto.Instance.ExportSubjectPublicKeyInfo();
        var sharedSecretSpan = sharedSecret.Span;
        Span<byte> buffer = stackalloc byte[sharedSecretSpan.Length + publicKey.Length];
        sharedSecretSpan.CopyTo(buffer);
        publicKey.CopyTo(buffer[sharedSecretSpan.Length..]);
        Span<byte> serverId = stackalloc byte[SHA1.HashSizeInBytes];
        SHA1.HashData(buffer, serverId);
        var negative = (serverId[0] & 0x80) == 0x80;

        if (negative)
            TwosComplement(serverId);

        var serverIdComplement = Convert.ToHexString(serverId).TrimStart('0');

        if (negative)
            serverIdComplement = "-" + serverIdComplement;

        var url = $"{SessionServer}?username={profile.Username}&serverId={serverIdComplement}";

        if (PreventProxyConnections)
            url += "&ip=" + player.RemoteEndPoint;

        var response = await Client.GetAsync(url, cancellationToken);

        if (response.StatusCode is HttpStatusCode.NoContent)
            return null;

        if (await response.Content.ReadFromJsonAsync<GameProfile>(cancellationToken) is not { } onlineProfile)
            return null;

        var key = player.IdentifiedKey;

        if (key is null || key.Revision != IdentifiedKeyRevision.LinkedV2Revision)
            return onlineProfile;

        return key.AddUuid(onlineProfile.Id) ? onlineProfile : null;
    }

    private static void TwosComplement(Span<byte> data)
    {
        var carry = true;

        for (var i = data.Length - 1; i >= 0; i--)
        {
            data[i] = unchecked((byte)~data[i]);

            if (!carry)
                continue;

            carry = data[i] == 0xFF;
            data[i]++;
        }
    }
}
