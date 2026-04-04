using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;
using Void.IntegrationTests.Infrastructure.Exceptions;

namespace Void.IntegrationTests.Infrastructure.Extensions;

public static class ContainerExtensions
{
    extension(IContainer container)
    {
        public async Task RunCommandAsync(string[] command, CancellationToken cancellationToken = default)
        {
            var execResult = await container.ExecAsync(command, cancellationToken);

            if (execResult.ExitCode != 0)
                throw new IntegrationTestException($"Exit code {execResult.ExitCode}\nCommand {string.Join(" ", command)}\nSTDOUT:\n{execResult.Stdout}\nSTDERR:\n{execResult.Stderr}");
        }

        public async Task RunCommandAsync(string command, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var processGroupLeaderFilePath = $"/tmp/run-command-process-group-{Guid.NewGuid():N}";
            var heredocMarker = $"RUN_COMMAND_{Guid.NewGuid():N}_{Guid.NewGuid():N}";

            var wrappedCommand = $$"""
            set -euo pipefail
            trap 'rm -f "{{processGroupLeaderFilePath}}"' EXIT
            printf '%s\n' "$$" > "{{processGroupLeaderFilePath}}"

            bash -seuo pipefail <<'{{heredocMarker}}'
            {{command.ReplaceLineEndings("\n")}}
            {{heredocMarker}}
            """;

            var executionTask = container.ExecAsync(["setsid", "bash", "-c", wrappedCommand]);

            try
            {
                var executionResult = await executionTask.WaitAsync(cancellationToken);

                if (executionResult.ExitCode is not 0)
                {
                    throw new IntegrationTestException($"Exit code {executionResult.ExitCode}\nCommand {command}\nSTDOUT:\n{executionResult.Stdout}\nSTDERR:\n{executionResult.Stderr}");
                }

                return;
            }
            catch (OperationCanceledException)
            {
                try
                {
                    var killResult = await container.ExecAsync(["bash", "-c", $$"""
                    for currentAttempt in {1..30}; do
                        if [ -s "{{processGroupLeaderFilePath}}" ]; then
                            break
                        fi
                        sleep 0.2
                    done

                    if [ ! -e "{{processGroupLeaderFilePath}}" ]; then
                        exit 2
                    fi

                    processGroupLeaderProcessId="$(cat "{{processGroupLeaderFilePath}}")"

                    if [ -z "$processGroupLeaderProcessId" ]; then
                        exit 2
                    fi

                    if ! kill -0 -- "-$processGroupLeaderProcessId" 2>/dev/null; then
                        exit 3
                    fi

                    kill -KILL -- "-$processGroupLeaderProcessId"
                    """]);

                    switch (killResult.ExitCode)
                    {
                        case 2:
                        case 3:
                        {
                            try
                            {
                                var executionResult = await executionTask.WaitAsync(TimeSpan.FromSeconds(5));

                                if (executionResult.ExitCode is not 0)
                                {
                                    throw new IntegrationTestException($"Exit code {executionResult.ExitCode}\nCommand {command}\nSTDOUT:\n{executionResult.Stdout}\nSTDERR:\n{executionResult.Stderr}");
                                }

                                return;
                            }
                            catch (TimeoutException)
                            {
                                throw new IntegrationTestException($"Cancellation was requested, but the process group leader was not available yet.\nCommand {command}");
                            }
                        }
                        case not 0:
                            throw new IntegrationTestException($"Failed to kill the started process group.\nCommand {command}\nKill exit code: {killResult.ExitCode}\nKill STDOUT:\n{killResult.Stdout}\nKill STDERR:\n{killResult.Stderr}");
                    }

                    try
                    {
                        await executionTask.WaitAsync(TimeSpan.FromSeconds(5));
                    }
                    catch (TimeoutException)
                    {
                        throw new IntegrationTestException($"Cancellation was requested, but the started process group did not exit.\nCommand {command}\nKill exit code: {killResult.ExitCode}\nKill STDOUT:\n{killResult.Stdout}\nKill STDERR:\n{killResult.Stderr}");
                    }

                    throw new OperationCanceledException(cancellationToken);
                }
                finally
                {
                    await container.ExecAsync(["rm", "-f", processGroupLeaderFilePath]);
                }
            }
        }
    }
}
