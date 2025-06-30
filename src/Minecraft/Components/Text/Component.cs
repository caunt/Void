using System.Text.Json;
using System.Text.Json.Nodes;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Components.Text.Properties;
using Void.Minecraft.Components.Text.Properties.Content;
using Void.Minecraft.Components.Text.Serializers;
using Void.Minecraft.Nbt;
using Void.Minecraft.Nbt.Serializers.String;
using Void.Minecraft.Network;

namespace Void.Minecraft.Components.Text;

public record Component(IContent Content, Children Children, Formatting Formatting, Interactivity Interactivity)
{
    /// <summary>
    /// Represents a default component initialized with empty text content, default children, formatting, and
    /// interactivity.
    /// </summary>
    public static Component Default { get; } = new(new TextContent(string.Empty), Children.Default, Formatting.Default, Interactivity.Default);

    public static implicit operator Component(string text) => DeserializeLegacy(text);

    /// <summary>
    /// Returns a string representation of the component.
    /// </summary>
    public string AsText => SerializeLegacy('\0');

    /// <summary>
    /// Reads data from a buffer and deserializes it based on the specified protocol version.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for reading data.</typeparam>
    /// <param name="buffer">This parameter is the source from which data is read and processed for deserialization.</param>
    /// <param name="protocolVersion">This parameter indicates the version of the protocol to determine the deserialization method.</param>
    /// <returns>The method returns a Component object created from the deserialized data.</returns>
    public static Component ReadFrom<TBuffer>(ref TBuffer buffer, ProtocolVersion protocolVersion) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        if (protocolVersion <= ProtocolVersion.MINECRAFT_1_20_2)
        {
            var value = buffer.ReadString();
            var node = (JsonNode?)null;

            try
            {
                node = JsonNode.Parse(value);
            }
            catch (JsonException)
            {
                // Ignore, not a json
            }

            if (node is null)
                return DeserializeLegacy(value);
            else
                return DeserializeJson(node);
        }
        else
        {
            return DeserializeNbt(buffer.ReadTag());
        }
    }

    /// <summary>
    /// Reads data from a buffer and deserializes it into a Component based on the protocol version.
    /// </summary>
    /// <param name="buffer">The buffer containing the data to be read and deserialized into a Component.</param>
    /// <param name="protocolVersion">Indicates the version of the protocol to determine the deserialization method.</param>
    /// <returns>Returns a Component object created from the data in the buffer.</returns>
    public static Component ReadFrom(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion <= ProtocolVersion.MINECRAFT_1_20_2)
        {
            var value = buffer.ReadString();
            var node = (JsonNode?)null;

            try
            {
                node = JsonNode.Parse(value);
            }
            catch (JsonException)
            {
                // Ignore, not a json
            }

            if (node is null)
                return DeserializeLegacy(value);
            else
                return DeserializeJson(node);
        }
        else
        {
            return DeserializeNbt(buffer.ReadTag());
        }
    }

    /// <summary>
    /// Writes serialized data to a buffer based on the specified protocol version.
    /// </summary>
    /// <param name="buffer">The buffer is used to store serialized data in a specific format.</param>
    /// <param name="protocolVersion">The protocol version determines the format of the serialized data.</param>
    public void WriteTo(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion <= ProtocolVersion.MINECRAFT_1_20_2)
            buffer.WriteString(SerializeJson().ToString());
        else
            buffer.WriteTag(SerializeNbt());
    }

    /// <summary>
    /// Writes serialized data to a buffer based on the specified protocol version.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for writing data.</typeparam>
    /// <param name="buffer">This parameter is the destination where the serialized data will be written.</param>
    /// <param name="protocolVersion">This parameter indicates the version of the protocol to determine the serialization format.</param>
    public void WriteTo<TBuffer>(ref TBuffer buffer, ProtocolVersion protocolVersion) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        if (protocolVersion <= ProtocolVersion.MINECRAFT_1_20_2)
            buffer.WriteString(SerializeJson().ToString());
        else
            buffer.WriteTag(SerializeNbt());
    }

    /// <summary>
    /// Deserializes a string into a Component object using legacy serialization.
    /// </summary>
    /// <param name="source">The string representation of the Component to be deserialized.</param>
    /// <param name="prefix">The character used to identify properties in the string representation.</param>
    /// <returns>Returns a deserialized Component object.</returns>
    public static Component DeserializeLegacy(string source, char prefix = '&')
    {
        return ComponentLegacySerializer.Deserialize(source, prefix);
    }

    /// <summary>
    /// Deserializes a JSON node into a Component object based on the specified protocol version.
    /// </summary>
    /// <param name="source">The JSON node containing the data to be deserialized into a Component.</param>
    /// <param name="protocolVersion">Specifies the version of the protocol to be used during deserialization.</param>
    /// <returns>Returns a Component object created from the provided JSON data.</returns>
    public static Component DeserializeJson(JsonNode source)
    {
        return ComponentJsonSerializer.Deserialize(source);
    }

    /// <summary>
    /// Deserializes an NBT tag into a Component object using a specified protocol version.
    /// </summary>
    /// <param name="tag">The NBT tag that contains the data to be deserialized into a Component.</param>
    /// <param name="protocolVersion">Specifies the version of the protocol to be used during deserialization.</param>
    /// <returns>Returns a Component object created from the provided NBT tag.</returns>
    public static Component DeserializeNbt(NbtTag tag)
    {
        return ComponentNbtSerializer.Deserialize(tag);
    }

    /// <summary>
    /// Deserializes a string representation of SNBT into a Component object using a specified protocol version.
    /// </summary>
    /// <param name="source">The string representation of SNBT that needs to be converted into a Component.</param>
    /// <param name="protocolVersion">Specifies the version of the protocol to be used during the deserialization process.</param>
    /// <returns>Returns a Component object created from the deserialized SNBT data.</returns>
    public static Component DeserializeSnbt(string source)
    {
        return DeserializeNbt(NbtStringSerializer.Deserialize(source));
    }

    /// <summary>
    /// Serializes the current object to Legacy format using a specified protocol version.
    /// </summary>
    /// <param name="prefix">Specifies the prefix of the formatting codes to be used during serialization.</param>
    /// <returns>Returns a Legacy string representing the serialized object.</returns>
    public string SerializeLegacy(char prefix = '&')
    {
        return ComponentLegacySerializer.Serialize(this, prefix);
    }

    /// <summary>
    /// Serializes the current component to JSON format.
    /// </summary>
    /// <returns>Returns a JsonNode representing the serialized component.</returns>
    public JsonNode SerializeJson()
    {
        return ComponentJsonSerializer.Serialize(this);
    }

    /// <summary>
    /// Serializes the current component into NBT format.
    /// </summary>
    /// <returns>Returns the serialized NBT tag representation of the component.</returns>
    public NbtTag SerializeNbt()
    {
        return ComponentNbtSerializer.Serialize(this);
    }

    /// <summary>
    /// Serializes the current component into SNBT format.
    /// </summary>
    /// <returns>Returns the serialized data in SNBT format.</returns>
    public string SerializeSnbt()
    {
        return NbtStringSerializer.Serialize(SerializeNbt());
    }

    /// <summary>
    /// Converts the object to its string representation by serializing it to SNBT format using the latest protocol
    /// version.
    /// </summary>
    /// <returns>Returns the string representation of the serialized NBT data.</returns>
    public override string ToString() => SerializeSnbt();
}
