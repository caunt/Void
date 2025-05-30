﻿using Void.Proxy.Api.Network;

namespace Void.Proxy.Api.Commands;

public interface ICommandService
{
    public ICommandDispatcher Dispatcher { get; }

    public ValueTask<CommandExecutionResult> ExecuteAsync(ICommandSource source, string command, CancellationToken cancellationToken = default);
    public ValueTask<CommandExecutionResult> ExecuteAsync(ICommandSource source, string command, Side origin, CancellationToken cancellationToken = default);
    public ValueTask<string[]> CompleteAsync(string input, ICommandSource source, CancellationToken cancellationToken = default);
}
