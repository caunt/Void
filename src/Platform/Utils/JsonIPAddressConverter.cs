using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Void.Proxy.Utils;

public class JsonIpAddressConverter : JsonConverter<IPAddress>
{
    public override IPAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new InvalidDataException();

        Span<char> charData = stackalloc char[45];
        int count;

        if (reader.HasValueSequence)
        {
            Span<byte> bytes = stackalloc byte[45];
            reader.ValueSequence.CopyTo(bytes);
            count = Encoding.UTF8.GetChars(bytes[..(int)reader.ValueSequence.Length], charData);
        }
        else
        {
            count = Encoding.UTF8.GetChars(reader.ValueSpan, charData);
        }

        return !IPAddress.TryParse(charData[..count], out var value) ? throw new InvalidDataException() : value;
    }

    public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
    {
        Span<char> data = stackalloc char[value.AddressFamily == AddressFamily.InterNetwork ? 15 : 45];

        if (!value.TryFormat(data, out var charsWritten))
            throw new JsonException($"IPAddress [{value}] could not be written to JSON.");

        writer.WriteStringValue(data[..charsWritten]);
    }
}
