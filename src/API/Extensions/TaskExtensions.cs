using Microsoft.Extensions.Logging;

namespace Void.Proxy.Api.Extensions;

public static class TaskExtensions
{
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
                    logger.LogError("{Message} (Canceled)\n{Exception}\n{StackTrace}", message, exception.StackTrace, stackTrace);
                }
            }

        }, TaskContinuationOptions.NotOnRanToCompletion);
    }

    public static ValueTask CatchExceptions(this ValueTask task, ILogger logger, string message)
    {
        return new ValueTask(task.AsTask().CatchExceptions(logger, message));
    }
}