package voidclient.agent;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import org.junit.Assert;
import org.junit.Test;
import org.objectweb.asm.Opcodes;
import org.objectweb.asm.tree.ClassNode;
import org.objectweb.asm.tree.FieldNode;
import org.objectweb.asm.tree.MethodInsnNode;
import org.objectweb.asm.tree.MethodNode;

public final class ConnectionFailureDiscoveryTest {
    @Test
    public void discoversNativeListenerAcrossRenamingAndLibraryRelocation() {
        for (String prefix : new String[] { "", "relocated/" }) {
            Map<String, ClassNode> types = new HashMap<String, ClassNode>();
            ClassNode network = type(types, "unknown/Network", prefix + "io/netty/channel/SimpleChannelInboundHandler");
            network.fields.add(new FieldNode(0, "listener", "Lunknown/Listener;", null, null));
            MethodNode disconnected = new MethodNode(0, "closed", "()V", null, null);
            disconnected.instructions.add(new MethodInsnNode(Opcodes.INVOKEINTERFACE, "unknown/Listener", "rejected", "(Lunknown/Reason;)V", true));
            network.methods.add(disconnected);
            ClassNode listener = type(types, "unknown/Listener", "java/lang/Object");
            listener.access = Opcodes.ACC_INTERFACE;
            ClassNode implementation = type(types, "unknown/LoginListener", "java/lang/Object");
            implementation.interfaces.add(listener.name);
            implementation.methods.add(new MethodNode(Opcodes.ACC_PUBLIC, "rejected", "(Lunknown/Reason;)V", null, null));
            Map<String, List<String>> callbacks = ConnectionFailureDiscovery.discover(types);
            Assert.assertEquals("rejected(Lunknown/Reason;)V", callbacks.get(implementation.name).get(0));
        }
    }

    @Test
    public void discoversRejectionWhenCallerLivesOutsideTheTransport() {
        Map<String, ClassNode> types = new HashMap<String, ClassNode>();
        ClassNode network = type(types, "unknown/Network", "io/netty/channel/SimpleChannelInboundHandler");
        network.fields.add(new FieldNode(0, "listener", "Lunknown/Listener;", null, null));
        network.fields.add(new FieldNode(0, "reason", "Lunknown/Reason;", null, null));
        ClassNode listener = type(types, "unknown/Listener", "java/lang/Object");
        listener.access = Opcodes.ACC_INTERFACE;
        listener.methods.add(new MethodNode(Opcodes.ACC_PUBLIC | Opcodes.ACC_ABSTRACT, "closed", "(Lunknown/Reason;)V", null, null));
        ClassNode implementation = type(types, "unknown/LoginListener", "java/lang/Object");
        implementation.interfaces.add(listener.name);
        implementation.methods.add(new MethodNode(Opcodes.ACC_PUBLIC, "closed", "(Lunknown/Reason;)V", null, null));
        Assert.assertEquals("closed(Lunknown/Reason;)V", ConnectionFailureDiscovery.discover(types).get(implementation.name).get(0));
    }

    private static ClassNode type(Map<String, ClassNode> types, String name, String parent) {
        ClassNode type = new ClassNode();
        type.name = name;
        type.superName = parent;
        types.put(name, type);
        return type;
    }
}
