---
title: Configuration
description: Learn how to define configuration for your plugin.
---

Void exposes a configuration API that allows you to define in-file configuration options for your plugin. 

## Defining Configuration
To define configuration for your plugin, create any class with any parameters and default values.
```csharp
class MySettings
{
    public bool Enable { get; set; } = true;
    public string? Name { get; set; } = "DefaultName";
    public int Count { get; set; } = 0;
    public Dictionary<string, string> Mapping { get; set; } = new() 
    { 
        ["key"] = "value" 
    };
}
```

## Loading Configuration
Inject `IConfigurationService` into your plugin or service constructor.  
Then use `GetAsync<T>()` method to get the configuration instance.
```csharp
public class MyPlugin(IConfigurationService configs) : IPlugin
{
    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        // This event is fired when any plugin is being loaded

        // Skip all other plugins load events except ours
        if (@event.Plugin != this)
            return;
        
        var settings = await configs.GetAsync<MySettings>(cancellationToken);
    }
}
```

Now you can find this configuration file at `configs/MyPlugin/MySettings.toml`

```toml
// configs/MyPlugin/MySettings.toml
Enable = true
Name = "DefaultName"
Count = 0

[Mapping]
key = "value"
```

:::note
Same instance of `MySettings` will be returned no matter how many times you call `.GetAsync<MySettings>()`.  

This instance will be automatically updated when the configuration file is changed.  
All changes to the instance will be automatically saved to the configuration file.
:::

## Decoration
It is possible to decorate configuration properties with `ConfigurationProperty` attribute.
```csharp
public class MySettings
{
    [ConfigurationProperty(Name = "ExampleName", InlineComment = "comment at the end of setting", PrecedingComment = "comment before setting")]
    public bool ExampleValue { get; set; } = true;
}
```

You can also set a custom name for the configuration file with the `RootConfiguration` attribute.
```csharp
[RootConfiguration("settings")]
public class MySettings
{
    public bool ExampleValue { get; set; } = true;
}
```

## Keyed Configuration
If you want to save multiple instances of the same configuration, you can use keyed configuration.  
To save different settings per-player, use the following example:
```csharp
public class MyPlugin(IConfigurationService configs) : IPlugin
{
    [Subscribe]
    public async ValueTask OnPlayerConnected(PlayerConnectedEvent @event)
    {
        var settings = await configs.GetAsync<MySettings>(@event.Player.Profile.Username);
    }
}
```
