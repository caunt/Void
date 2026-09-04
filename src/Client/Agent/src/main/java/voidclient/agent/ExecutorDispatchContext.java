package voidclient.agent;

import java.util.concurrent.Executor;
import java.util.concurrent.atomic.AtomicBoolean;
import java.util.concurrent.atomic.AtomicInteger;

final class ExecutorDispatchContext {
    private static final AtomicInteger InFlight = new AtomicInteger();
    private static final ThreadLocal<Boolean> Executing = new ThreadLocal<Boolean>();

    private ExecutorDispatchContext() {
    }

    static void execute(Executor executor, final Runnable command) {
        final AtomicBoolean released = new AtomicBoolean();
        InFlight.incrementAndGet();

        try {
            executor.execute(new Runnable() {
                @Override
                public void run() {
                    Boolean previousExecuting = Executing.get();
                    Executing.set(Boolean.TRUE);

                    try {
                        command.run();
                    } finally {
                        restore(Executing, previousExecuting);
                        release(released);
                    }
                }
            });
        } catch (RuntimeException exception) {
            release(released);
            throw exception;
        } catch (Error error) {
            release(released);
            throw error;
        }
    }

    static boolean isExecuting() {
        return Boolean.TRUE.equals(Executing.get());
    }

    static boolean isSchedulingCallback() {
        return InFlight.get() > 0 && !isExecuting();
    }

    private static void release(AtomicBoolean released) {
        if (released.compareAndSet(false, true))
            InFlight.decrementAndGet();
    }

    private static void restore(ThreadLocal<Boolean> context, Boolean previous) {
        if (previous == null)
            context.remove();
        else
            context.set(previous);
    }
}
