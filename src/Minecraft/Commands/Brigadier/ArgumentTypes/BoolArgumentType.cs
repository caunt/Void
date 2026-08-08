using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Suggestion;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

/// <summary>Contains a parsed Boolean argument.</summary>
/// <param name="Value">The parsed value.</param>
public record BoolArgumentValue(bool Value) : IArgumentValue;
/// <summary>Parses the case-insensitive Boolean tokens <c>true</c> and <c>false</c>.</summary>
public record BoolArgumentType : IArgumentType
{
    /// <inheritdoc/>
    public IEnumerable<string> Examples => ["true", "false"];

    private BoolArgumentType()
    {
    }

    /// <summary>Creates a Boolean argument type.</summary>
    /// <returns>A new Boolean argument type.</returns>
    public static BoolArgumentType Bool()
    {
        return new BoolArgumentType();
    }

    /// <summary>Gets a parsed Boolean argument from a context.</summary>
    /// <param name="context">The command context.</param>
    /// <param name="name">The argument name.</param>
    /// <returns>The parsed Boolean.</returns>
    public static bool GetBool(CommandContext context, string name)
    {
        return context.GetArgument<bool>(name);
    }

    /// <inheritdoc/>
    public IArgumentValue Parse(StringReader reader)
    {
        return new BoolArgumentValue(reader.ReadBoolean());
    }

    /// <inheritdoc/>
    public virtual async ValueTask<Suggestions> ListSuggestionsAsync(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken)
    {
        foreach (var example in Examples)
        {
            if (example.StartsWith(builder.Remaining, StringComparison.OrdinalIgnoreCase))
                builder.Suggest(example);
        }

        return await builder.BuildAsync(cancellationToken);
    }
}
