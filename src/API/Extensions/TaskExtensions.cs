using Serilog;

namespace Void.Proxy.API.Extensions;

public static class TaskExtensions
{
    public static Task CatchExceptions(this Task task)
    {
        return task.ContinueWith(completedTask =>
        {
            if (completedTask.Exception != null)
                Log.Logger.Fatal("Unhandled task Exception:\n{Exception}", completedTask.Exception.InnerException);
        }, TaskContinuationOptions.OnlyOnFaulted);
    }

    public static Task CatchExceptions(this ValueTask task)
    {
        return task.AsTask().CatchExceptions();
    }
}