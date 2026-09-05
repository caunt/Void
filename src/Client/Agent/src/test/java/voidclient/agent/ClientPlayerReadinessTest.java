package voidclient.agent;

import com.mojang.authlib.GameProfile;
import java.util.ArrayList;
import java.util.List;
import org.junit.Assert;
import org.junit.Test;

public final class ClientPlayerReadinessTest {
    @Test
    public void requiresExpectedPlayerInstalledInTheCurrentWorld() {
        Tracker.initialize(null, "expected");
        Client client = new Client();
        World current = new World();
        Player player = new Player("expected", current);
        Tracker.registerPlayer(player);
        Assert.assertNull(Tracker.activePlayer(client));
        client.player = player;
        client.world = current;
        Assert.assertNull(Tracker.activePlayer(client));
        current.players.add(player);
        Assert.assertSame(player, Tracker.activePlayer(client));
        client.world = new World();
        Assert.assertNull(Tracker.activePlayer(client));
        client.world = current;
        client.player = new Player("unrelated", current);
        current.players.add(client.player);
        Assert.assertNull(Tracker.activePlayer(client));
    }

    @Test
    public void baseConstructorObservationDoesNotCompleteReadiness() {
        Tracker.initialize(null, "expected");
        final Client client = new Client();
        client.world = new World();
        Player player = new Player("expected", client.world) {
            {
                Assert.assertNull(Tracker.activePlayer(client));
            }
        };
        client.player = player;
        client.world.players.add(player);
        Assert.assertSame(player, Tracker.activePlayer(client));
    }

    public static class Client {
        public Player player;
        public World world;
    }
    public static class World {
        public final List<Player> players = new ArrayList<Player>();
    }
    public static class Player {
        public final GameProfile profile;
        public final World world;
        public Player(String name, World world) {
            this.profile = new GameProfile(name);
            this.world = world;
            Tracker.registerPlayer(this);
        }
    }
}
