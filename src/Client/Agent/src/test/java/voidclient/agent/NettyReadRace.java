package voidclient.agent;

import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.nio.NioSocketChannel;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.ServerSocket;
import java.nio.channels.SelectionKey;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.TimeUnit;

public final class NettyReadRace {
    public static boolean run(boolean enableAgain) throws Exception {
        NioEventLoopGroup group = new NioEventLoopGroup(1);
        InspectableChannel channel = new InspectableChannel();
        CountDownLatch eventLoopEntered = new CountDownLatch(1);
        CountDownLatch readDisabled = new CountDownLatch(1);

        try (ServerSocket server = new ServerSocket(0, 1, InetAddress.getLoopbackAddress())) {
            group.register(channel).sync();
            channel.connect(new InetSocketAddress(InetAddress.getLoopbackAddress(), server.getLocalPort())).sync();
            io.netty.util.concurrent.Future<?> transition = channel.eventLoop().submit(() -> {
                eventLoopEntered.countDown();
                try {
                    if (!readDisabled.await(5, TimeUnit.SECONDS))
                        throw new AssertionError("Read disable was not issued");
                    if (enableAgain)
                        channel.config().setAutoRead(true);
                } catch (InterruptedException exception) {
                    Thread.currentThread().interrupt();
                    throw new AssertionError(exception);
                }
            });

            if (!eventLoopEntered.await(5, TimeUnit.SECONDS))
                throw new AssertionError("Event loop did not start");

            // The off-loop disable queues a clear. Re-enable inside the current
            // event-loop task before that queued clear gets a chance to execute.
            channel.config().setAutoRead(false);
            readDisabled.countDown();
            transition.sync();
            return channel.eventLoop().submit(() -> channel.readInterested()).sync().getNow();
        } finally {
            readDisabled.countDown();
            channel.close().sync();
            group.shutdownGracefully(0, 0, TimeUnit.SECONDS).sync();
        }
    }

    private static final class InspectableChannel extends NioSocketChannel {
        boolean readInterested() {
            return (selectionKey().interestOps() & SelectionKey.OP_READ) != 0;
        }
    }
}
