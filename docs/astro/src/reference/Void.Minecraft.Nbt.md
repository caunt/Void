# <a id="Void_Minecraft_Nbt"></a> Namespace Void.Minecraft.Nbt

### Namespaces

 [Void.Minecraft.Nbt.Snbt](Void.Minecraft.Nbt.Snbt.md)

 [Void.Minecraft.Nbt.Tags](Void.Minecraft.Nbt.Tags.md)

### Classes

 [ArrayTag<T\>](Void.Minecraft.Nbt.ArrayTag\-1.md)

Base class for NBT tags that contain a fixed-size array of numeric types.

 [BoolTag](Void.Minecraft.Nbt.BoolTag.md)

A tag that contains a single 8-bit integer value.

 [BufferedTagWriter](Void.Minecraft.Nbt.BufferedTagWriter.md)

Provides a <xref href="Void.Minecraft.Nbt.TagWriter" data-throw-if-not-resolved="false"></xref> object that writes to an internal buffer instead of a <xref href="System.IO.Stream" data-throw-if-not-resolved="false"></xref> object, which then can be retrieved as
an array of bytes or written directly to a stream. This is especially convenient when creating packets to be sent over a network, where the size of
the packet must be pre-determined before sending.

 [ByteArrayTag](Void.Minecraft.Nbt.ByteArrayTag.md)

A tag whose value is a contiguous sequence of 8-bit integers.

 [ByteTag](Void.Minecraft.Nbt.ByteTag.md)

A tag that contains a single 8-bit integer value.

 [CompoundTag](Void.Minecraft.Nbt.CompoundTag.md)

Top-level tag that acts as a container for other <b>named</b> tags.

 [TagBuilder.Context](Void.Minecraft.Nbt.TagBuilder.Context.md)

Represents the context of a single "level" of depth into a <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> AST.

 [DoubleTag](Void.Minecraft.Nbt.DoubleTag.md)

A tag that contains a single IEEE-754 double-precision floating point number.

 [EndTag](Void.Minecraft.Nbt.EndTag.md)

Represents the end of <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref>.

 [FloatTag](Void.Minecraft.Nbt.FloatTag.md)

A tag that contains a single IEEE-754 single-precision floating point number.

 [IntArrayTag](Void.Minecraft.Nbt.IntArrayTag.md)

A tag whose value is a contiguous sequence of 32-bit integers.

 [IntTag](Void.Minecraft.Nbt.IntTag.md)

A tag that contains a single 32-bit integer value.

 [ListTag](Void.Minecraft.Nbt.ListTag.md)

Represents a collection of a tags.

 [LongArrayTag](Void.Minecraft.Nbt.LongArrayTag.md)

A tag whose value is a contiguous sequence of 64-bit integers.

 [LongTag](Void.Minecraft.Nbt.LongTag.md)

A tag that contains a single 64-bit integer value.

 [NbtFile](Void.Minecraft.Nbt.NbtFile.md)

Provides static convenience methods for working with NBT-formatted files, including both reading and writing.

 [NbtReader](Void.Minecraft.Nbt.NbtReader.md)

 [NbtTag<T\>](Void.Minecraft.Nbt.NbtTag\-1.md)

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

 [NumberExtensions](Void.Minecraft.Nbt.NumberExtensions.md)

Contains extension methods dealing with endianness of numeric types.

 [NumericTag<T\>](Void.Minecraft.Nbt.NumericTag\-1.md)

Abstract base class for <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> types that contain a single numeric value.

 [ShortTag](Void.Minecraft.Nbt.ShortTag.md)

A tag that contains a single 16-bit integer value.

 [StringTag](Void.Minecraft.Nbt.StringTag.md)

A tag the contains a UTF-8 string.

 [Tag](Void.Minecraft.Nbt.Tag.md)

Abstract base class that all NBT tags inherit from.

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Provides a mechanism for easily building a tree of NBT objects by handling the intermediate step of creating tags, allowing the direct adding of their
equivalent values.

<p></p>

All methods return the <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance itself, allowing for easily chaining calls to build a document.

 [TagEventArgs](Void.Minecraft.Nbt.TagEventArgs.md)

Arguments supplied with tag-related events.

 [TagHandledEventArgs](Void.Minecraft.Nbt.TagHandledEventArgs.md)

Arguments supplied when an event that can be handled by an event subscriber.

 [TagIO](Void.Minecraft.Nbt.TagIO.md)

Abstract base class for the <xref href="Void.Minecraft.Nbt.TagReader" data-throw-if-not-resolved="false"></xref> and <xref href="Void.Minecraft.Nbt.TagWriter" data-throw-if-not-resolved="false"></xref> classes, providing shared functionality.

 [TagReader](Void.Minecraft.Nbt.TagReader.md)

Provides methods for reading NBT data from a stream.

 [TagWriter](Void.Minecraft.Nbt.TagWriter.md)

Provides methods for writing NBT tags to a stream.

 [VarInt](Void.Minecraft.Nbt.VarInt.md)

Provides static methods for reading and writing variable-length integers that are up to 5 bytes from both streams and buffers.

 [VarLong](Void.Minecraft.Nbt.VarLong.md)

Provides static methods for reading and writing variable-length integers that are up to 10 bytes from both streams and buffers.

### Enums

 [CompressionType](Void.Minecraft.Nbt.CompressionType.md)

Describes compression formats supported by the NBT specification.

 [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Describes the specification to use for reading/writing.

 [NbtFormatOptions](Void.Minecraft.Nbt.NbtFormatOptions.md)

 [NbtTagType](Void.Minecraft.Nbt.NbtTagType.md)

 [TagType](Void.Minecraft.Nbt.TagType.md)

Strongly-typed numerical constants that are prefixed to tags to denote their type.

### Delegates

 [TagReaderCallback<T\>](Void.Minecraft.Nbt.TagReaderCallback\-1.md)

Handler for events used with the <xref href="Void.Minecraft.Nbt.TagReader" data-throw-if-not-resolved="false"></xref> class.

