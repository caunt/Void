---
title: Text Formatting
description: Learn how to work with text components.
---

Most of the places where you send a text value to Minecraft, it accepts [**text components**](https://minecraft.wiki/w/Text_component_format). This includes chat messages, titles, action bars, and more.  
With text components, you can create rich text messages with formatting, colors and events.

## Basic Usage
Likely you have already used text components in your plugin with `SendChatMessageAsync`.
```csharp
public class MyScopedService(IPlayerContext context)
{
    public async ValueTask SendMessageAsync(CancellationToken cancellationToken)
    {
        // This method actually accepts a Component class
        await context.Player.SendChatMessageAsync("Hello, world!", cancellationToken);
    }
}
```

## Formatting Codes
It is easy to format text with the Minecraft [formatting codes](https://minecraft.fandom.com/wiki/Formatting_codes) using the & symbol.
```csharp
public class MyScopedService(IPlayerContext context)
{
    public async ValueTask SendMessageAsync(CancellationToken cancellationToken)
    {
        await context.Player.SendChatMessageAsync("&croses are red, &9violets are blue, &6honey is sweet, &dand so are you", cancellationToken);
    }
}
```

## Manually Creating Text Components
You can also create text components manually. This is useful if you want to create a more complex message with multiple components or custom behavior.
```csharp
public class MyScopedService(IPlayerContext context)
{
    public async ValueTask SendMessageAsync(CancellationToken cancellationToken)
    {
        var component = Component.Default with
        {
            Content = new TextContent("Open Void repository"),
            Interactivity = Interactivity.Default with
            {
                ClickEvent = new ClickEvent(new OpenUrl("https://github.com/caunt/Void")),
                HoverEvent = new HoverEvent(new ShowText("Click this text")),
            }
        };

        await context.Player.SendChatMessageAsync(component, cancellationToken);
    }
}
```

## Converting Components
Components can be converted from and to many different formats like Legacy, Json, Nbt and Snbt
```csharp
public class MyScopedService(IPlayerContext context)
{
    public async ValueTask SendMessageAsync(CancellationToken cancellationToken)
    {
        Component component = "&croses are red, &9violets are blue, &6honey is sweet, &dand so are you";

        var legacy = component.SerializeLegacy(prefix: '&');
        var json = component.SerializeJson();
        var nbt = component.SerializeNbt();
        var snbt = component.SerializeSnbt();

        component = Component.DeserializeLegacy(legacy, prefix: '&');
        component = Component.DeserializeJson(json);
        component = Component.DeserializeNbt(nbt);
        component = Component.DeserializeSnbt(snbt);
    }
}
```

## Extract Text from Components
You can also extract only text from components.
```csharp
public class MyScopedService(IPlayerContext context)
{
    public async ValueTask SendMessageAsync(CancellationToken cancellationToken)
    {
        Component component = "&croses are red, &9violets are blue, &6honey is sweet, &dand so are you";

        // The value is: roses are red, violets are blue, honey is sweet, and so are you
        string text = component.AsText;
    }
}
```