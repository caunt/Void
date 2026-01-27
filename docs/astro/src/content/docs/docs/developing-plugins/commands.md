---
title: Commands
description: Learn how to define and listen to commands.
---

Commands help users and administrators interact with the proxy and plugins.

## Defining a command
Inject `ICommandService` service to begin working with commands.  
Commands are registered with the `Register` method in [**Mojang Brigadier**](https://github.com/Mojang/brigadier/)-like style.

```csharp
class MyService(ICommandService commands, ILogger<MyService> logger)
{
    public void RegisterCommands()
    {
        commands.Register(builder => builder
            .Literal("hello")
            .Executes(HelloAsync));
    }

    private async ValueTask<int> HelloAsync(CommandContext context, CancellationToken cancellationToken)
    {
        // Commands are triggered by console, plugins, players, or anything
        if (context.Source is IPlayer player)
        {
            await player.SendChatMessageAsync("Hello, World!", cancellationToken);
        }
        else
        {
            logger.LogInformation("Hello, World!");
        }
        
        return 0;
    }
}
```

## Manually triggering a command
You can trigger a command execution manually with `ExecuteAsync` method.
```csharp
class MyService(ICommandService commands) : ICommandSource
{
    public async ValueTask TriggerCommandAsync(CancellationToken cancellationToken)
    {
        ICommandSource commandSource = this;
        string command = "hello";

        await commands.ExecuteAsync(commandSource, command, cancellationToken);
    }
}
```

## Command suggestions
You can request suggestions for a command with the `CompleteAsync` method.
```csharp
class MyService(ICommandService commands) : ICommandSource
{
    public async ValueTask SuggestCommandAsync(CancellationToken cancellationToken)
    {
        ICommandSource commandSource = this;
        string command = "he";
        
        var variants = await commands.CompleteAsync(command, commandSource, cancellationToken);
        // variants value is string[] { "hello" }
    }
}
```

## Command arguments
You can define optional command arguments with `Argument` method.
```csharp
class MyService(ICommandService commands, ILogger<MyService> logger)
{
    public void RegisterCommands()
    {
        commands.Register(builder => builder
            .Literal("hello")
            .Then(builder => builder
                .Argument("name", Arguments.String())
                .Executes(HelloAsync)));
    }

    private async ValueTask<int> HelloAsync(CommandContext context, CancellationToken cancellationToken)
    {
        // Arguments are optional
        if (!context.TryGetArgument<string>("name", out var name))
            name = "World";
        
        if (context.Source is IPlayer player)
        {
            await player.SendChatMessageAsync($"Hello, {name}!", cancellationToken);
        }
        else
        {
            logger.LogInformation($"Hello, {name}!");
        }

        return 0;
    }
}
```

## Complex Command Example
```csharp
class MyService(ICommandService commands, ILogger<MyService> logger)
{
    // Register a `slot` command to change player held item slot
    public void RegisterCommands()
    {
        commands.Register(builder => builder
            .Literal("slot")
            .Executes(SlotCommandAsync)
            .Then(builder => builder
                .Argument("index", Arguments.Integer())
                .Executes(SlotCommandAsync)));
    }

    private async ValueTask<int> SlotCommandAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (context.Source is not IPlayer player)
        {
            logger.LogInformation("This command can be executed only by a player");
            return 1;
        }

        if (!context.TryGetArgument<int>("index", out var slot))
        {
            // If the slot argument is not provided, we will use a random one
            slot = Random.Shared.Next(0, 9);
        }

        await player.SendChatMessageAsync($"Your held item slot is changed to {slot}", cancellationToken);
        await player.SendPacketAsync(new SetHeldItemClientboundPacket { Slot = slot }, cancellationToken);
        return 0;
    }
}
```