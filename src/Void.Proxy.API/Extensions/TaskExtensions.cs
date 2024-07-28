namespace Void.Proxy.API.Extensions;

public static class TaskExtensions
{
    public static Task CatchExceptions(this Task task)
    {
        return task.ContinueWith(completedTask =>
            {
                if (completedTask.Exception != null)
                    Console.WriteLine("Unhandled task Exception:\n" + completedTask.Exception.InnerException);
            },
            TaskContinuationOptions.OnlyOnFaulted);
    }

    public static Task CatchExceptions(this ValueTask task)
    {
        return task.AsTask()
            .CatchExceptions();
    }
}