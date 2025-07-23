namespace Void.Tests.Integration.Sides.Servers;

using System;

[Flags]
public enum PaperPlugins
{
    None = 0,
    ViaVersion = 1,
    ViaBackwards = 2,
    All = ViaVersion | ViaBackwards
}
