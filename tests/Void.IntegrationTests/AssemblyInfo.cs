using Xunit;

#if DEBUG
[assembly: CaptureConsole]
#endif

[assembly: CollectionBehavior(MaxParallelThreads = 4)]
