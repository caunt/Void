﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Suggestion;

namespace Void.Minecraft.Commands.Brigadier.Arguments;

public record BoolArgumentType : IArgumentType<bool>
{
    public IEnumerable<string> Examples => ["true", "false"];

    private BoolArgumentType()
    {
    }

    public static BoolArgumentType Bool()
    {
        return new BoolArgumentType();
    }

    public static bool GetBool(CommandContext context, string name)
    {
        return context.GetArgument<bool>(name);
    }

    public bool Parse(StringReader reader)
    {
        return reader.ReadBoolean();
    }

    public virtual async ValueTask<Suggestions> ListSuggestionsAsync(CommandContext context, SuggestionsBuilder builder)
    {
        foreach (var example in Examples)
        {
            if (example.StartsWith(builder.RemainingLowerCase))
                builder.Suggest(example);
        }

        return await builder.BuildAsync();
    }
}
