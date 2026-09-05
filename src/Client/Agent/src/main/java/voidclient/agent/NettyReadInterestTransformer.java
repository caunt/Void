package voidclient.agent;

import java.lang.instrument.ClassFileTransformer;
import java.security.ProtectionDomain;
import org.objectweb.asm.ClassReader;
import org.objectweb.asm.ClassWriter;
import org.objectweb.asm.Opcodes;
import org.objectweb.asm.tree.ClassNode;
import org.objectweb.asm.tree.FrameNode;
import org.objectweb.asm.tree.InsnList;
import org.objectweb.asm.tree.InsnNode;
import org.objectweb.asm.tree.JumpInsnNode;
import org.objectweb.asm.tree.LabelNode;
import org.objectweb.asm.tree.MethodInsnNode;
import org.objectweb.asm.tree.MethodNode;
import org.objectweb.asm.tree.VarInsnNode;

final class NettyReadInterestTransformer implements ClassFileTransformer {
    static final String ChannelName = "io/netty/channel/nio/AbstractNioChannel";

    @Override
    public byte[] transform(ClassLoader loader, String className, Class<?> classBeingRedefined, ProtectionDomain protectionDomain, byte[] classFileBuffer) {
        if (!ChannelName.equals(className))
            return null;

        ClassNode type = new ClassNode();
        new ClassReader(classFileBuffer).accept(type, 0);
        boolean changed = false;
        for (MethodNode method : type.methods) {
            if (!"clearReadPending0".equals(method.name) || !"()V".equals(method.desc))
                continue;

            // Netty queues this operation when autoRead is disabled off-loop.
            // A newer enable can run first on the event loop; the stale clear
            // must not then remove OP_READ from an auto-reading connection.
            LabelNode clear = new LabelNode();
            InsnList guard = new InsnList();
            guard.add(new VarInsnNode(Opcodes.ALOAD, 0));
            guard.add(new MethodInsnNode(Opcodes.INVOKEVIRTUAL, ChannelName, "config", "()Lio/netty/channel/ChannelConfig;", false));
            guard.add(new MethodInsnNode(Opcodes.INVOKEINTERFACE, "io/netty/channel/ChannelConfig", "isAutoRead", "()Z", true));
            guard.add(new JumpInsnNode(Opcodes.IFEQ, clear));
            guard.add(new InsnNode(Opcodes.RETURN));
            guard.add(clear);
            guard.add(new FrameNode(Opcodes.F_SAME, 0, null, 0, null));
            method.instructions.insert(guard);
            changed = true;
        }

        if (!changed)
            return null;

        ClassWriter writer = new ClassWriter(ClassWriter.COMPUTE_MAXS);
        type.accept(writer);
        return writer.toByteArray();
    }
}
