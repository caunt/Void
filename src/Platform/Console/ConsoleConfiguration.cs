using System.CommandLine;

namespace Void.Proxy.Console;

public record ConsoleConfiguration(bool HasTerminal, Command RootCommand);
