package voidclient.agent;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Executor;
import org.junit.Assert;
import org.junit.Test;

public final class ExecutorDispatchContextTest {
    @Test
    public void suppressesSchedulingCallbacksUntilQueuedCommandExecutes() {
        final List<Runnable> commands = new ArrayList<Runnable>();
        final int[] callbacks = new int[1];
        final int[] executions = new int[1];
        Executor executor = new Executor() {
            @Override
            public void execute(Runnable command) {
                applyInstrumentedCallback(callbacks);
                commands.add(command);
            }
        };

        ExecutorDispatchContext.execute(executor, new Runnable() {
            @Override
            public void run() {
                applyInstrumentedCallback(callbacks);
                executions[0]++;
            }
        });

        Assert.assertEquals(0, callbacks[0]);
        Assert.assertEquals(0, executions[0]);
        Assert.assertEquals(1, commands.size());

        applyInstrumentedCallback(callbacks);
        Assert.assertEquals(0, callbacks[0]);

        commands.get(0).run();

        Assert.assertEquals(1, callbacks[0]);
        Assert.assertEquals(1, executions[0]);
        Assert.assertFalse(ExecutorDispatchContext.isExecuting());
        Assert.assertFalse(ExecutorDispatchContext.isSchedulingCallback());
    }

    @Test
    public void suppressesCallbacksFromOtherThreadsWhileCommandIsQueued() throws Exception {
        final List<Runnable> commands = new ArrayList<Runnable>();
        final int[] callbacks = new int[1];
        Executor executor = new Executor() {
            @Override
            public void execute(Runnable command) {
                commands.add(command);
            }
        };

        ExecutorDispatchContext.execute(executor, new Runnable() {
            @Override
            public void run() {
                applyInstrumentedCallback(callbacks);
            }
        });

        Thread callbackThread = new Thread(new Runnable() {
            @Override
            public void run() {
                applyInstrumentedCallback(callbacks);
            }
        });
        callbackThread.start();
        callbackThread.join();

        Assert.assertEquals(0, callbacks[0]);

        commands.get(0).run();

        Assert.assertEquals(1, callbacks[0]);
        Assert.assertFalse(ExecutorDispatchContext.isExecuting());
        Assert.assertFalse(ExecutorDispatchContext.isSchedulingCallback());
    }

    @Test
    public void allowsCommandRunInlineDuringScheduling() {
        final int[] callbacks = new int[1];
        final int[] executions = new int[1];
        Executor executor = new Executor() {
            @Override
            public void execute(Runnable command) {
                applyInstrumentedCallback(callbacks);
                command.run();
                applyInstrumentedCallback(callbacks);
            }
        };

        ExecutorDispatchContext.execute(executor, new Runnable() {
            @Override
            public void run() {
                applyInstrumentedCallback(callbacks);
                executions[0]++;
            }
        });

        Assert.assertEquals(2, callbacks[0]);
        Assert.assertEquals(1, executions[0]);
        Assert.assertFalse(ExecutorDispatchContext.isExecuting());
        Assert.assertFalse(ExecutorDispatchContext.isSchedulingCallback());
    }

    @Test
    public void restoresContextAfterExecutorFailure() {
        Executor executor = new Executor() {
            @Override
            public void execute(Runnable command) {
                throw new IllegalStateException("rejected");
            }
        };

        try {
            ExecutorDispatchContext.execute(executor, new Runnable() {
                @Override
                public void run() {
                }
            });
            Assert.fail("Expected executor failure");
        } catch (IllegalStateException exception) {
            Assert.assertEquals("rejected", exception.getMessage());
        }

        Assert.assertFalse(ExecutorDispatchContext.isExecuting());
        Assert.assertFalse(ExecutorDispatchContext.isSchedulingCallback());
    }

    @Test
    public void restoresNestedAndSequentialExecutionContexts() {
        final int[] executions = new int[1];
        Executor inlineExecutor = new Executor() {
            @Override
            public void execute(Runnable command) {
                command.run();
            }
        };

        ExecutorDispatchContext.execute(inlineExecutor, new Runnable() {
            @Override
            public void run() {
                Assert.assertTrue(ExecutorDispatchContext.isExecuting());
                ExecutorDispatchContext.execute(new Executor() {
                    @Override
                    public void execute(Runnable command) {
                        Assert.assertTrue(ExecutorDispatchContext.isExecuting());
                        command.run();
                    }
                }, new Runnable() {
                    @Override
                    public void run() {
                        Assert.assertTrue(ExecutorDispatchContext.isExecuting());
                        executions[0]++;
                    }
                });
                Assert.assertTrue(ExecutorDispatchContext.isExecuting());
                executions[0]++;
            }
        });

        ExecutorDispatchContext.execute(inlineExecutor, new Runnable() {
            @Override
            public void run() {
                executions[0]++;
            }
        });

        Assert.assertEquals(3, executions[0]);
        Assert.assertFalse(ExecutorDispatchContext.isExecuting());
        Assert.assertFalse(ExecutorDispatchContext.isSchedulingCallback());
    }

    private static void applyInstrumentedCallback(int[] callbacks) {
        if (!ExecutorDispatchContext.isSchedulingCallback())
            callbacks[0]++;
    }
}
