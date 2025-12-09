using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using Void.Proxy.Api.Commands;

namespace Void.Proxy.Api.Console;

public interface IConsoleService : ICommandSource
{
    /// <summary>
    /// Asynchronously processes a single incoming command in the terminal until cancellation is requested. Do not call this method directly.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will stop processing current command if the token is
    /// canceled.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public ValueTask HandleCommandsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to retrieve the value associated with the specified option from the parsed command-line arguments.
    /// </summary>
    /// <typeparam name="TValue">The type of the value associated with the option.</typeparam>
    /// <param name="option">The option for which to retrieve the value. Cannot be <see langword="null"/>.</param>
    /// <param name="value">When this method returns <see langword="true"/>, contains the value associated with the specified option. When
    /// this method returns <see langword="false"/>, the value is <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the value for the specified option was successfully retrieved; otherwise, <see
    /// langword="false"/>.</returns>
    public bool TryGetOptionValue<TValue>(Option<TValue> option, [MaybeNullWhen(false)] out TValue value);

    /// <summary>
    /// Retrieves the value associated with the specified option from the parsed command-line arguments.
    /// </summary>
    /// <remarks>Use this method to access the value of a specific option after parsing command-line
    /// arguments. If the option is not present in the arguments, the method returns <see langword="null"/>.</remarks>
    /// <typeparam name="TValue">The type of the value associated with the option.</typeparam>
    /// <param name="option">The option whose value is to be retrieved. Cannot be <see langword="null"/>.</param>
    /// <returns>The value of the specified option if it is present; otherwise, <see langword="null"/>.</returns>
    public TValue? GetOptionValue<TValue>(Option<TValue> option);

    /// <summary>
    /// Retrieves the value of the specified option, ensuring that it is set from the parsed command-line arguments.
    /// </summary>
    /// <typeparam name="TValue">The type of the value associated with the option.</typeparam>
    /// <param name="option">The option whose value is to be retrieved. Cannot be <see langword="null"/>.</param>
    /// <returns>The value of the specified option.</returns>
    public TValue GetRequiredOptionValue<TValue>(Option<TValue> option);

    /// <summary>
    /// Ensures that the specified command-line option is discovered by platform.
    /// </summary>
    /// <remarks>This method is typically used to track options that have been identified during
    /// processing.</remarks>
    /// <param name="option">The option to be marked as discovered. Cannot be <see langword="null"/>.</param>
    public void EnsureOptionDiscovered(Option option);
}
