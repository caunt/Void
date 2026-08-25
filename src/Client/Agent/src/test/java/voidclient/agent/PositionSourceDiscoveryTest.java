package voidclient.agent;

import com.mojang.authlib.GameProfile;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.lang.reflect.Field;
import java.util.List;
import org.junit.Assert;
import org.junit.Test;
import org.objectweb.asm.ClassReader;
import org.objectweb.asm.Opcodes;
import org.objectweb.asm.tree.AbstractInsnNode;
import org.objectweb.asm.tree.ClassNode;
import org.objectweb.asm.tree.FieldInsnNode;
import org.objectweb.asm.tree.InsnNode;
import org.objectweb.asm.tree.MethodInsnNode;
import org.objectweb.asm.tree.MethodNode;
import org.objectweb.asm.tree.VarInsnNode;

public final class PositionSourceDiscoveryTest {
    @Test
    public void discoversLegacyCoordinateFieldsWithoutNames() {
        ClassNode type = createDistanceType(false);

        PositionSource[] sources = PositionSourceDiscovery.findPositionSources(type);

        Assert.assertNotNull(sources);
        Assert.assertEquals("a", sources[0].name);
        Assert.assertEquals("b", sources[1].name);
        Assert.assertEquals("c", sources[2].name);
        Assert.assertFalse(sources[0].method);
    }

    @Test
    public void discoversModernCoordinateGettersWithoutNames() {
        ClassNode type = createDistanceType(true);

        PositionSource[] sources = PositionSourceDiscovery.findPositionSources(type);

        Assert.assertNotNull(sources);
        Assert.assertEquals("a", sources[0].name);
        Assert.assertEquals("b", sources[1].name);
        Assert.assertEquals("c", sources[2].name);
        Assert.assertTrue(sources[0].method);
    }

    @Test
    public void normalizesPlayerHeightFromAnUnnamedBoundingBox() {
        Double minimumY = Tracker.boundingBoxMinimumY(new SyntheticEntity(), 20.5, -7.5);

        Assert.assertNotNull(minimumY);
        Assert.assertEquals(100.0, minimumY.doubleValue(), 0.0);
    }

    @Test
    public void identifiesDuplicatePlayersByProfileIdentity() {
        Assert.assertTrue(Tracker.samePlayerIdentity("first-id", "old-name", "first-id", "new-name"));
        Assert.assertTrue(Tracker.samePlayerIdentity(null, "same-name", "second-id", "same-name"));
        Assert.assertFalse(Tracker.samePlayerIdentity("first-id", "same-name", "second-id", "same-name"));
        Assert.assertFalse(Tracker.samePlayerIdentity(null, null, null, null));
    }

    @Test
    public void identifiesPlayerCollectionsFromGenericElementStructure() throws ReflectiveOperationException {
        Field players = SyntheticWorld.class.getDeclaredField("players");
        Field entities = SyntheticWorld.class.getDeclaredField("entities");

        Assert.assertTrue(Tracker.hasPlayerElementType(players.getGenericType(), 0));
        Assert.assertFalse(Tracker.hasPlayerElementType(entities.getGenericType(), 0));
    }

    @Test
    public void instrumentsStructurallyIdentifiedPlayerConstructors() throws IOException {
        byte[] instrumented = PlayerTransformer.instrumentPlayerConstructors(classBytes(SyntheticPlayer.class));

        Assert.assertNotNull(instrumented);
        ClassNode type = new ClassNode();
        new ClassReader(instrumented).accept(type, 0);
        boolean registrationFound = false;

        for (MethodNode method : type.methods) {
            for (AbstractInsnNode instruction = method.instructions.getFirst(); instruction != null; instruction = instruction.getNext()) {
                if (instruction instanceof MethodInsnNode) {
                    MethodInsnNode invocation = (MethodInsnNode) instruction;
                    registrationFound |= "voidclient/agent/Tracker".equals(invocation.owner) && "registerPlayer".equals(invocation.name);
                }
            }
        }

        Assert.assertTrue(registrationFound);
        Assert.assertNull(PlayerTransformer.instrumentPlayerConstructors(classBytes(SyntheticEntity.class)));
    }

    private static byte[] classBytes(Class<?> type) throws IOException {
        String resourceName = "/" + type.getName().replace('.', '/') + ".class";

        try (InputStream stream = type.getResourceAsStream(resourceName)) {
            Assert.assertNotNull(stream);
            ByteArrayOutputStream output = new ByteArrayOutputStream();
            byte[] buffer = new byte[4096];
            int count;

            while ((count = stream.read(buffer)) >= 0)
                output.write(buffer, 0, count);

            return output.toByteArray();
        }
    }

    private static ClassNode createDistanceType(boolean methods) {
        ClassNode type = new ClassNode();
        type.name = "x/y";
        MethodNode distance = new MethodNode(Opcodes.ACC_PUBLIC, "z", "(DDD)D", null, null);
        appendCoordinate(distance, type.name, "a", 1, methods);
        appendCoordinate(distance, type.name, "b", 3, methods);
        distance.instructions.add(new InsnNode(Opcodes.DADD));
        appendCoordinate(distance, type.name, "c", 5, methods);
        distance.instructions.add(new InsnNode(Opcodes.DADD));
        distance.instructions.add(new InsnNode(Opcodes.DRETURN));
        type.methods.add(distance);
        return type;
    }

    private static void appendCoordinate(MethodNode distance, String owner, String name, int variable, boolean methods) {
        distance.instructions.add(new VarInsnNode(Opcodes.ALOAD, 0));

        if (methods)
            distance.instructions.add(new MethodInsnNode(Opcodes.INVOKEVIRTUAL, owner, name, "()D", false));
        else
            distance.instructions.add(new FieldInsnNode(Opcodes.GETFIELD, owner, name, "D"));

        distance.instructions.add(new VarInsnNode(Opcodes.DLOAD, variable));
        distance.instructions.add(new InsnNode(Opcodes.DSUB));
        distance.instructions.add(new InsnNode(Opcodes.DUP2));
        distance.instructions.add(new InsnNode(Opcodes.DMUL));
    }

    private static final class SyntheticEntity {
        private final SyntheticBox value = new SyntheticBox();
    }

    private static final class SyntheticPlayer {
        private final GameProfile profile = new GameProfile();
    }

    private static final class SyntheticWorld {
        private List<SyntheticPlayer> players;
        private List<SyntheticEntity> entities;
    }

    private static final class SyntheticBox {
        private final double first = 20.2;
        private final double second = 100.0;
        private final double third = -7.8;
        private final double fourth = 20.8;
        private final double fifth = 101.8;
        private final double sixth = -7.2;
    }
}
