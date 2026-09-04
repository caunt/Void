package voidclient.agent;

import java.lang.instrument.ClassFileTransformer;
import java.net.URL;
import java.security.CodeSource;
import java.security.ProtectionDomain;
import org.objectweb.asm.ClassReader;
import org.objectweb.asm.ClassWriter;
import org.objectweb.asm.Opcodes;
import org.objectweb.asm.tree.AbstractInsnNode;
import org.objectweb.asm.tree.ClassNode;
import org.objectweb.asm.tree.InsnList;
import org.objectweb.asm.tree.MethodInsnNode;
import org.objectweb.asm.tree.MethodNode;
import org.objectweb.asm.tree.VarInsnNode;

final class GameAutomationTransformer implements ClassFileTransformer {
    private static final String ControllerName = "voidclient/agent/GameAutomationController";
    @Override
    public byte[] transform(ClassLoader loader, String className, Class<?> classBeingRedefined,
                            ProtectionDomain protectionDomain, byte[] classFileBuffer) {
        if (className == null || className.startsWith("java/") || className.startsWith("voidclient/agent/"))
            return null;

        try {
            CodeSource codeSource = protectionDomain == null ? null : protectionDomain.getCodeSource();
            URL location = codeSource == null ? null : codeSource.getLocation();
            GameAutomationIndex.IndexedCode index = GameAutomationIndex.index(location);

            if (index == null)
                return null;

            GameAutomationController.registerIndex(loader, index);
            boolean client = index.plan.clientClassName.equals(className)
                || index.plan.chatDriverClassName.equals(className);

            if (!client)
                return null;

            ClassNode type = new ClassNode();
            new ClassReader(classFileBuffer).accept(type, 0);
            boolean changed = instrument(type, "applyClient");

            if (!changed)
                return null;

            if (client)
                System.err.println("Void client agent instrumented client-thread driver " + className);

            ClassWriter writer = new ClassWriter(ClassWriter.COMPUTE_MAXS);
            type.accept(writer);
            return writer.toByteArray();
        } catch (Throwable exception) {
            GameAutomationController.recordIndexFailure(className, exception.getClass().getName() + ": " + exception.getMessage());
            return null;
        }
    }

    private static boolean instrument(ClassNode type, String controllerMethod) {
        boolean changed = false;

        for (MethodNode method : type.methods) {
            if ("<init>".equals(method.name) || "<clinit>".equals(method.name)
                || (method.access & (Opcodes.ACC_STATIC | Opcodes.ACC_ABSTRACT | Opcodes.ACC_NATIVE)) != 0
                || alreadyInstrumented(method, controllerMethod))
                continue;

            method.instructions.insert(controllerCall(controllerMethod));
            changed = true;
        }

        return changed;
    }

    private static InsnList controllerCall(String controllerMethod) {
        InsnList call = new InsnList();
        call.add(new VarInsnNode(Opcodes.ALOAD, 0));
        call.add(new MethodInsnNode(Opcodes.INVOKESTATIC, ControllerName, controllerMethod, "(Ljava/lang/Object;)V", false));
        return call;
    }

    private static boolean alreadyInstrumented(MethodNode method, String controllerMethod) {
        for (AbstractInsnNode instruction = method.instructions.getFirst(); instruction != null; instruction = instruction.getNext()) {
            if (instruction instanceof MethodInsnNode) {
                MethodInsnNode call = (MethodInsnNode) instruction;

                if (ControllerName.equals(call.owner) && controllerMethod.equals(call.name))
                    return true;
            }
        }

        return false;
    }
}
