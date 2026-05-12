# <a id="Void_Minecraft_Commands_Brigadier_Arguments"></a> Class Arguments

Namespace: [Void.Minecraft.Commands.Brigadier](Void.Minecraft.Commands.Brigadier.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class Arguments
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Arguments](Void.Minecraft.Commands.Brigadier.Arguments.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Arguments_Bool"></a> Bool\(\)

```csharp
public static BoolArgumentType Bool()
```

#### Returns

 [BoolArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.BoolArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_Arguments_Double_System_Double_System_Double_"></a> Double\(double, double\)

```csharp
public static DoubleArgumentType Double(double min = -1.7976931348623157E+308, double max = 1.7976931348623157E+308)
```

#### Parameters

`min` [double](https://learn.microsoft.com/dotnet/api/system.double)

`max` [double](https://learn.microsoft.com/dotnet/api/system.double)

#### Returns

 [DoubleArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.DoubleArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_Arguments_Float_System_Single_System_Single_"></a> Float\(float, float\)

```csharp
public static FloatArgumentType Float(float min = -3.4028235E+38, float max = 3.4028235E+38)
```

#### Parameters

`min` [float](https://learn.microsoft.com/dotnet/api/system.single)

`max` [float](https://learn.microsoft.com/dotnet/api/system.single)

#### Returns

 [FloatArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.FloatArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_Arguments_GetBool_Void_Minecraft_Commands_Brigadier_Context_CommandContext_System_String_"></a> GetBool\(CommandContext, string\)

```csharp
public static bool GetBool(CommandContext context, string name)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_Arguments_GetDouble_Void_Minecraft_Commands_Brigadier_Context_CommandContext_System_String_"></a> GetDouble\(CommandContext, string\)

```csharp
public static double GetDouble(CommandContext context, string name)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Void_Minecraft_Commands_Brigadier_Arguments_GetFloat_Void_Minecraft_Commands_Brigadier_Context_CommandContext_System_String_"></a> GetFloat\(CommandContext, string\)

```csharp
public static float GetFloat(CommandContext context, string name)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Void_Minecraft_Commands_Brigadier_Arguments_GetInteger_Void_Minecraft_Commands_Brigadier_Context_CommandContext_System_String_"></a> GetInteger\(CommandContext, string\)

```csharp
public static int GetInteger(CommandContext context, string name)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Commands_Brigadier_Arguments_GetLong_Void_Minecraft_Commands_Brigadier_Context_CommandContext_System_String_"></a> GetLong\(CommandContext, string\)

```csharp
public static long GetLong(CommandContext context, string name)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Commands_Brigadier_Arguments_GetString_Void_Minecraft_Commands_Brigadier_Context_CommandContext_System_String_"></a> GetString\(CommandContext, string\)

```csharp
public static string GetString(CommandContext context, string name)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Arguments_GreedyString"></a> GreedyString\(\)

```csharp
public static StringArgumentType GreedyString()
```

#### Returns

 [StringArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.StringArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_Arguments_Integer_System_Int32_System_Int32_"></a> Integer\(int, int\)

```csharp
public static IntegerArgumentType Integer(int min = -2147483648, int max = 2147483647)
```

#### Parameters

`min` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`max` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [IntegerArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IntegerArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_Arguments_Long_System_Int64_System_Int64_"></a> Long\(long, long\)

```csharp
public static LongArgumentType Long(long min = -9223372036854775808, long max = 9223372036854775807)
```

#### Parameters

`min` [long](https://learn.microsoft.com/dotnet/api/system.int64)

`max` [long](https://learn.microsoft.com/dotnet/api/system.int64)

#### Returns

 [LongArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.LongArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_Arguments_String"></a> String\(\)

```csharp
public static StringArgumentType String()
```

#### Returns

 [StringArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.StringArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_Arguments_Word"></a> Word\(\)

```csharp
public static StringArgumentType Word()
```

#### Returns

 [StringArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.StringArgumentType.md)

