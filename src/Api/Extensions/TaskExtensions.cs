using Microsoft.Extensions.Logging;

namespace Void.Proxy.Api.Extensions;

public static class TaskExtensions
{
    /// <summary>
    /// Attaches a continuation that logs failures or cancellations from <paramref name="task"/> and returns that continuation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The continuation is registered with <see cref="TaskContinuationOptions.NotOnRanToCompletion"/>, so it runs only when the antecedent task faults or is canceled.
    /// </para>
    /// <para>
    /// For faulted tasks, this method logs <c>completedTask.Exception.InnerException</c>. For canceled tasks, it calls <see cref="Task.Wait()"/> to materialize <see cref="TaskCanceledException"/>, then logs that exception together with the call-site stack trace captured when this method was invoked.
    /// </para>
    /// <para>
    /// The returned <see cref="Task"/> represents only the logging continuation, not the original task result. Because it does not rethrow antecedent failures, awaiting the returned task usually completes successfully unless logging itself throws.
    /// </para>
    /// </remarks>
    /// <param name="task">The task to observe for non-successful completion.</param>
    /// <param name="logger">The logger used to emit error messages.</param>
    /// <param name="message">A message prefix included in each error log entry.</param>
    /// <returns>
    /// A continuation task that completes after any required logging for <paramref name="task"/> has finished.
    /// </returns>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="task"/> is <see langword="null"/> and this extension method is invoked as a static call.
    /// </exception>
    /// <example>
    /// <code>
    /// _ = backgroundTask.CatchExceptions(logger, "Background processing failed");
    /// </code>
    /// </example>
    /// <see cref="Task.ContinueWith(Action{Task}, TaskContinuationOptions)"/>
    /// <seealso cref="CatchExceptions(ValueTask, ILogger, string)"/>
    public static Task CatchExceptions(this Task task, ILogger logger, string message)
    {
        var stackTrace = Environment.StackTrace;

        return task.ContinueWith(completedTask =>
        {
            if (completedTask.Exception is not null)
                logger.LogError("{Message}:\n{Exception}", message, completedTask.Exception.InnerException);

            if (completedTask.IsCanceled)
            {
                try
                {
                    completedTask.Wait();
                }
                catch (TaskCanceledException exception)
                {
                    logger.LogError("{Message} (Canceled)\n{Exception}\n{StackTrace}", message, exception, stackTrace);
                }
            }

        }, TaskContinuationOptions.NotOnRanToCompletion);
    }

    /// <summary>
    /// Converts <paramref name="task"/> to <see cref="Task"/> and applies <see cref="CatchExceptions(Task, ILogger, string)"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This overload delegates all logging behavior and error-handling semantics to <see cref="CatchExceptions(Task, ILogger, string)"/>.
    /// </para>
    /// <para>
    /// The returned <see cref="ValueTask"/> wraps the continuation task created for logging. As a result, awaiting it waits for logging completion rather than rethrowing the original failure from <paramref name="task"/>.
    /// </para>
    /// </remarks>
    /// <param name="task">The value task to observe for non-successful completion.</param>
    /// <param name="logger">The logger used to emit error messages.</param>
    /// <param name="message">A message prefix included in each error log entry.</param>
    /// <returns>
    /// A <see cref="ValueTask"/> wrapping the logging continuation produced from <paramref name="task"/>.
    /// </returns>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="logger"/> is <see langword="null"/> and logging is attempted.
    /// </exception>
    /// <example>
    /// <code>
    /// await valueTask.CatchExceptions(logger, "Link stop handler failed");
    /// </code>
    /// </example>
    /// <see cref="CatchExceptions(Task, ILogger, string)"/>
    /// <seealso cref="ValueTask.AsTask"/>
    public static ValueTask CatchExceptions(this ValueTask task, ILogger logger, string message)
    {
        return new ValueTask(task.AsTask().CatchExceptions(logger, message));
    }
}
