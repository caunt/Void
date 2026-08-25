package voidclient.agent;

import java.lang.instrument.ClassFileTransformer;
import java.security.ProtectionDomain;
import org.objectweb.asm.ClassReader;
import org.objectweb.asm.ClassWriter;
import org.objectweb.asm.Opcodes;
import org.objectweb.asm.tree.AbstractInsnNode;
import org.objectweb.asm.tree.ClassNode;
import org.objectweb.asm.tree.FieldNode;
import org.objectweb.asm.tree.InsnList;
import org.objectweb.asm.tree.MethodInsnNode;
import org.objectweb.asm.tree.MethodNode;
import org.objectweb.asm.tree.VarInsnNode;

final class PlayerTransformer implements ClassFileTransformer {
    private static final String GameProfileDescriptor = "Lcom/mojang/authlib/GameProfile;";
    private static final String TrackerName = "voidclient/agent/Tracker";

    @Override
    public byte[] transform(ClassLoader loader, String className, Class<?> classBeingRedefined, ProtectionDomain protectionDomain, byte[] classFileBuffer) {
        if (className == null || className.startsWith("java/") || className.startsWith("voidclient/agent/"))
            return null;

        try {
            return instrumentPlayerConstructors(classFileBuffer);
        } catch (Throwable exception) {
            System.err.println("Void client player instrumentation failed for " + className + ": " + exception.getMessage());
            return null;
        }
    }

    static byte[] instrumentPlayerConstructors(byte[] classFileBuffer) {
        ClassNode type = new ClassNode();
        new ClassReader(classFileBuffer).accept(type, 0);
        boolean declaresProfile = false;

        for (FieldNode field : type.fields) {
            if (GameProfileDescriptor.equals(field.desc)) {
                declaresProfile = true;
                break;
            }
        }

        if (!declaresProfile)
            return null;

        boolean changed = false;

        for (MethodNode method : type.methods) {
            if (!"<init>".equals(method.name))
                continue;

            for (AbstractInsnNode instruction = method.instructions.getFirst(); instruction != null; instruction = instruction.getNext()) {
                if (instruction.getOpcode() != Opcodes.RETURN)
                    continue;

                InsnList registration = new InsnList();
                registration.add(new VarInsnNode(Opcodes.ALOAD, 0));
                registration.add(new MethodInsnNode(Opcodes.INVOKESTATIC, TrackerName, "registerPlayer", "(Ljava/lang/Object;)V", false));
                method.instructions.insertBefore(instruction, registration);
                changed = true;
            }
        }

        if (!changed)
            return null;

        ClassWriter writer = new ClassWriter(ClassWriter.COMPUTE_MAXS);
        type.accept(writer);
        return writer.toByteArray();
    }
}
