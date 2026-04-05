using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;
using Void.IntegrationTests.Infrastructure.Exceptions;

namespace Void.IntegrationTests.Infrastructure.Extensions;

public static class ContainerExtensions
{
    extension(IContainer container)
    {
        public async Task ExpectTextAsync(string text, CancellationToken cancellationToken = default)
        {
            await container.ExpectTextAsync(text, since: DateTime.UtcNow, cancellationToken);
        }

        public async Task ExpectTextAsync(string text, DateTime since, CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var logs = await container.ReadLogsAsync(since, cancellationToken);

                if (logs.Any(line => line.Contains(text)))
                    return;

                await Task.Delay(100, cancellationToken);
            }
        }

        public async Task RunCommandAsync(string[] command, CancellationToken cancellationToken = default)
        {
            var execResult = await container.ExecAsync(command, cancellationToken);

            if (execResult.ExitCode != 0)
                throw new IntegrationTestException($"Exit code {execResult.ExitCode}\nCommand {string.Join(" ", command)}\nSTDOUT:\n{execResult.Stdout}\nSTDERR:\n{execResult.Stderr}");
        }

        public async Task RunCommandAsync(string command, CancellationToken cancellationToken = default, CancellationToken commandCancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (commandCancellationToken == default)
                commandCancellationToken = cancellationToken;

            var processGroupLeaderFilePath = $"/tmp/run-command-process-group-{Guid.NewGuid():N}";
            var heredocMarker = $"RUN_COMMAND_{Guid.NewGuid():N}_{Guid.NewGuid():N}";

            var executionTask = container.ExecAsync(["setsid", "--wait", "bash", "-c", $$"""
                set -euo pipefail
                trap 'rm -f "{{processGroupLeaderFilePath}}"' EXIT
                printf '%s\n' "$$" > "{{processGroupLeaderFilePath}}"
                bash -seuo pipefail <<'{{heredocMarker}}'
                {{command}}
                {{heredocMarker}}
                """.ReplaceLineEndings("\n")], cancellationToken);

            try
            {
                var executionResult = await executionTask.WaitAsync(commandCancellationToken);

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
                        """.ReplaceLineEndings("\n")], CancellationToken.None);

                    switch (killResult.ExitCode)
                    {
                        case 2 or 3:
                            {
                                try
                                {
                                    var executionResult = await executionTask.WaitAsync(TimeSpan.FromSeconds(5), CancellationToken.None);

                                    if (executionResult.ExitCode is not 0)
                                        throw new IntegrationTestException($"Exit code {executionResult.ExitCode}\nCommand {command}\nSTDOUT:\n{executionResult.Stdout}\nSTDERR:\n{executionResult.Stderr}");

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
                        await executionTask.WaitAsync(TimeSpan.FromSeconds(5), CancellationToken.None);
                    }
                    catch (TimeoutException)
                    {
                        throw new IntegrationTestException($"Cancellation was requested, but the started process group did not exit.\nCommand {command}\nKill exit code: {killResult.ExitCode}\nKill STDOUT:\n{killResult.Stdout}\nKill STDERR:\n{killResult.Stderr}");
                    }

                    throw new OperationCanceledException(commandCancellationToken);
                }
                finally
                {
                    await container.ExecAsync(["rm", "-f", processGroupLeaderFilePath], CancellationToken.None);
                }
            }
        }

        public async Task<IEnumerable<string>> ReadLogsAsync(DateTime since, CancellationToken cancellationToken = default)
        {
            var (standardOutput, standardError) = await container.GetLogsAsync(since, ct: cancellationToken);
            return Enumerate(standardOutput).Concat(Enumerate(standardError));
            static IEnumerable<string> Enumerate(string text) => text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(line => line.Trim('\r'));
        }

        public async Task<TimeSpan> GetTimeSinceLastLogAsync(CancellationToken cancellationToken = default)
        {
            async Task<bool> HasLogsWithinAgeAsync(DateTime snapshotTime, TimeSpan age)
            {
                var logs = await container.ReadLogsAsync(since: snapshotTime - age, cancellationToken);
                return logs.Any();
            }

            var searchPrecision = TimeSpan.FromMilliseconds(100);
            var snapshotTime = DateTime.UtcNow;
            var lowerAge = TimeSpan.Zero;
            var upperAge = TimeSpan.FromSeconds(1);

            cancellationToken.ThrowIfCancellationRequested();

            while (!await HasLogsWithinAgeAsync(snapshotTime, upperAge))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (upperAge >= snapshotTime - DateTime.UnixEpoch)
                    throw new InvalidOperationException("No logs were found.");

                upperAge += upperAge;
            }

            while (upperAge - lowerAge > searchPrecision)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var middleAge = lowerAge + (upperAge - lowerAge) / 2;

                if (await HasLogsWithinAgeAsync(snapshotTime, middleAge))
                {
                    upperAge = middleAge;
                }
                else
                {
                    lowerAge = middleAge;
                }
            }

            return upperAge + (DateTime.UtcNow - snapshotTime);
        }

        public async Task WaitForLogsSilenceAsync(TimeSpan requiredSilenceDuration, CancellationToken cancellationToken = default)
        {
            var checkInterval = TimeSpan.FromSeconds(1);
            var fixedSinceAnchor = DateTime.UtcNow - requiredSilenceDuration;

            var logs = await container.ReadLogsAsync(since: fixedSinceAnchor, cancellationToken);
            var previousLogCount = logs.Count();

            if (previousLogCount == 0)
                return;

            var lastLogTime = DateTime.UtcNow;

            while (DateTime.UtcNow - lastLogTime < requiredSilenceDuration)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(checkInterval, cancellationToken);

                logs = await container.ReadLogsAsync(since: fixedSinceAnchor, cancellationToken);
                var currentLogCount = logs.Count();

                if (currentLogCount > previousLogCount)
                {
                    lastLogTime = DateTime.UtcNow;
                    previousLogCount = currentLogCount;
                }
            }
        }

        public async Task<Memory<byte>> TakeScreenshotAsync(CancellationToken cancellationToken = default)
        {
            var fileName = $"/tmp/screenshot-{Guid.NewGuid():N}.png";

            await container.RunCommandAsync($"""
                    export DISPLAY=:99
                    import -display :99 -window root {fileName}
                    """, cancellationToken);

            try
            {
                return await container.ReadFileAsync(fileName, cancellationToken);
            }
            finally
            {
                await container.RunCommandAsync($"rm -f {fileName}", cancellationToken);
            }
        }
    }
}
