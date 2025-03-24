using Void.Minecraft.Components.Text.Properties;
using Void.Minecraft.Components.Text.Properties.Content;

namespace Void.Minecraft.Components.Text;

public record Component(IContent Content, Children Children, Formatting Formatting, Interactivity Interactivity);