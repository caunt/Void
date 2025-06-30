using System.Buffers;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Void.Proxy.Utils;

public class JsonIpEndPointConverter : JsonConverter<IPEndPoint>
{
    public override IPEndPoint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new InvalidDataException();

        Span<char> charData = stackalloc char[53];
        var count = Encoding.UTF8.GetChars(reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan, charData);

        var addressLength = count;
        var lastColonPos = charData.LastIndexOf(':');

        if (lastColonPos > 0)
        {
            if (charData[lastColonPos - 1] == ']')
                addressLength = lastColonPos;
            else if (charData[..lastColonPos].LastIndexOf(':') == -1)
                // Look to see if this is IPv4 with a port (IPv6 will have another colon)
                addressLength = lastColonPos;
        }

        if (!IPAddress.TryParse(charData[..addressLength], out var address))
            throw new InvalidDataException();

        uint port = 0;

        return addressLength == charData.Length || (uint.TryParse(charData[(addressLength + 1)..], NumberStyles.None, CultureInfo.InvariantCulture, out port) && port <= IPEndPoint.MaxPort) ? new IPEndPoint(address, (int)port) : throw new InvalidDataException();
    }

    public override void Write(Utf8JsonWriter writer, IPEndPoint value, JsonSerializerOptions options)
    {
        var isIpv6 = value.AddressFamily == AddressFamily.InterNetworkV6;
        // IPv6 endpoints require a larger buffer than IPv4 endpoints
        Span<char> data = stackalloc char[isIpv6 ? 53 : 21];
        var offset = 0;

        if (isIpv6)
        {
            data[0] = '[';
            offset++;
        }

        if (!value.Address.TryFormat(data[offset..], out var addressCharsWritten))
            throw new JsonException($"IPEndPoint [{value}] could not be written to JSON.");

        offset += addressCharsWritten;

        if (isIpv6)
            data[offset++] = ']';

        data[offset++] = ':';

        if (!value.Port.TryFormat(data[offset..], out var portCharsWritten))
            throw new JsonException($"IPEndPoint [{value}] could not be written to JSON.");

        writer.WriteStringValue(data[..(offset + portCharsWritten)]);
    }
}
