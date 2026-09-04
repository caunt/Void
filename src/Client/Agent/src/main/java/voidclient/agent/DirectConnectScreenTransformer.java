package voidclient.agent;

import java.io.IOException;
import java.io.InputStream;
import java.lang.instrument.ClassFileTransformer;
import java.security.ProtectionDomain;
import java.util.ArrayList;
import java.util.List;
import org.objectweb.asm.ClassReader;
import org.objectweb.asm.ClassWriter;
import org.objectweb.asm.Opcodes;
import org.objectweb.asm.tree.AbstractInsnNode;
import org.objectweb.asm.tree.ClassNode;
import org.objectweb.asm.tree.FieldInsnNode;
import org.objectweb.asm.tree.InsnList;
import org.objectweb.asm.tree.LdcInsnNode;
import org.objectweb.asm.tree.MethodInsnNode;
import org.objectweb.asm.tree.MethodNode;
import org.objectweb.asm.tree.VarInsnNode;

final class DirectConnectScreenTransformer implements ClassFileTransformer {
    private static final String DirectConnectTranslationKey = "selectServer.direct";
    private static final byte[] DirectConnectTranslationKeyBytes = DirectConnectTranslationKey.getBytes(java.nio.charset.StandardCharsets.US_ASCII);
    private static final String ControllerName = "voidclient/agent/DirectConnectController";

    @Override
    public byte[] transform(ClassLoader loader, String className, Class<?> classBeingRedefined, ProtectionDomain protectionDomain, byte[] classFileBuffer) {
        if (className == null || className.startsWith("java/") || className.startsWith("voidclient/agent/"))
            return null;

        try {
            ClassNode type = new ClassNode();
            new ClassReader(classFileBuffer).accept(type, 0);

            if (!containsDirectConnectTranslationKey(type))
                return null;

            DirectConnectPlan plan = discover(type);

            if (plan == null) {
                DirectConnectController.recordDiscoveryFailure(className, "No unique text/address/submit data flow was found");
                return null;
            }

            DirectConnectController.registerPlan(plan);
            boolean changed = instrumentRenderThreadEntries(type);

            if (!changed)
                return null;

            ClassWriter writer = new ClassWriter(ClassWriter.COMPUTE_MAXS);
            type.accept(writer);
            return writer.toByteArray();
        } catch (Throwable exception) {
            DirectConnectController.recordDiscoveryFailure(className, exception.getClass().getName() + ": " + exception.getMessage());
            return null;
        }
    }

    static DirectConnectPlan discover(ClassNode type) {
        List<DirectConnectPlan> candidates = new ArrayList<DirectConnectPlan>();

        for (MethodNode method : type.methods) {
            for (AbstractInsnNode instruction = method.instructions.getFirst(); instruction != null; instruction = instruction.getNext()) {
                if (!(instruction instanceof FieldInsnNode) || instruction.getOpcode() != Opcodes.PUTFIELD)
                    continue;

                FieldInsnNode addressField = (FieldInsnNode) instruction;

                if (!"Ljava/lang/String;".equals(addressField.desc))
                    continue;

                MethodInsnNode getter = findPreviousStringGetter(instruction, 12);

                if (getter == null)
                    continue;

                FieldInsnNode textField = findPreviousScreenField(getter, type.name, getter.owner, null, 12);
                FieldInsnNode serverDataField = findPreviousScreenField(textField, type.name, null, textField == null ? null : textField.name, 12);
                MethodInsnNode callback = findNextCallback(instruction, 16);
                FieldInsnNode callbackField = findPreviousScreenField(callback, type.name, callback == null ? null : callback.owner, null, 12);

                if (textField == null || serverDataField == null || callback == null || callbackField == null)
                    continue;

                MethodInsnNode setter = findSetter(type, textField, getter.owner);

                if (setter == null)
                    continue;

                DirectConnectPlan candidate = new DirectConnectPlan(type.name, textField.name, textField.desc,
                    setter.owner, setter.name, getter.owner, getter.name, serverDataField.name, serverDataField.desc,
                    addressField.owner, addressField.name, callbackField.name, callbackField.desc,
                    callback.owner, callback.name, callback.desc);

                if (!containsEquivalent(candidates, candidate))
                    candidates.add(candidate);
            }
        }

        return candidates.size() == 1 ? candidates.get(0) : null;
    }

    static boolean mightBeDirectConnectScreen(Class<?> type) {
        String resourceName = '/' + type.getName().replace('.', '/') + ".class";
        InputStream input = type.getResourceAsStream(resourceName);

        if (input == null)
            return false;

        try {
            byte[] buffer = new byte[8192];
            int read;
            int matched = 0;

            while ((read = input.read(buffer)) >= 0) {
                for (int offset = 0; offset < read; offset++) {
                    byte value = buffer[offset];

                    if (value == DirectConnectTranslationKeyBytes[matched]) {
                        matched++;

                        if (matched == DirectConnectTranslationKeyBytes.length)
                            return true;
                    } else {
                        matched = value == DirectConnectTranslationKeyBytes[0] ? 1 : 0;
                    }
                }
            }

            return false;
        } catch (IOException exception) {
            return false;
        } finally {
            try {
                input.close();
            } catch (IOException exception) {
                // The candidate probe is complete.
            }
        }
    }

    private static boolean containsDirectConnectTranslationKey(ClassNode type) {
        for (MethodNode method : type.methods) {
            for (AbstractInsnNode instruction = method.instructions.getFirst(); instruction != null; instruction = instruction.getNext()) {
                if (instruction instanceof LdcInsnNode && DirectConnectTranslationKey.equals(((LdcInsnNode) instruction).cst))
                    return true;
            }
        }

        return false;
    }

    private static MethodInsnNode findPreviousStringGetter(AbstractInsnNode start, int maximumInstructions) {
        AbstractInsnNode instruction = start.getPrevious();

        for (int inspected = 0; instruction != null && inspected < maximumInstructions; instruction = instruction.getPrevious()) {
            if (instruction.getOpcode() < 0)
                continue;

            inspected++;

            if (instruction instanceof MethodInsnNode && "()Ljava/lang/String;".equals(((MethodInsnNode) instruction).desc))
                return (MethodInsnNode) instruction;
        }

        return null;
    }

    private static MethodInsnNode findNextCallback(AbstractInsnNode start, int maximumInstructions) {
        AbstractInsnNode instruction = start.getNext();

        for (int inspected = 0; instruction != null && inspected < maximumInstructions; instruction = instruction.getNext()) {
            if (instruction.getOpcode() < 0)
                continue;

            inspected++;

            if (instruction instanceof MethodInsnNode) {
                MethodInsnNode method = (MethodInsnNode) instruction;

                if ("(Z)V".equals(method.desc) || "(ZI)V".equals(method.desc))
                    return method;
            }
        }

        return null;
    }

    private static FieldInsnNode findPreviousScreenField(AbstractInsnNode start, String screenOwner, String fieldTypeOwner, String excludedName, int maximumInstructions) {
        if (start == null)
            return null;

        AbstractInsnNode instruction = start.getPrevious();

        for (int inspected = 0; instruction != null && inspected < maximumInstructions; instruction = instruction.getPrevious()) {
            if (instruction.getOpcode() < 0)
                continue;

            inspected++;

            if (!(instruction instanceof FieldInsnNode) || instruction.getOpcode() != Opcodes.GETFIELD)
                continue;

            FieldInsnNode field = (FieldInsnNode) instruction;

            if (!screenOwner.equals(field.owner) || (excludedName != null && excludedName.equals(field.name)))
                continue;

            if (fieldTypeOwner == null || ("L" + fieldTypeOwner + ";").equals(field.desc))
                return field;
        }

        return null;
    }

    private static MethodInsnNode findSetter(ClassNode type, FieldInsnNode textField, String widgetOwner) {
        MethodInsnNode result = null;

        for (MethodNode method : type.methods) {
            for (AbstractInsnNode instruction = method.instructions.getFirst(); instruction != null; instruction = instruction.getNext()) {
                if (!(instruction instanceof MethodInsnNode))
                    continue;

                MethodInsnNode candidate = (MethodInsnNode) instruction;

                if (!widgetOwner.equals(candidate.owner) || !"(Ljava/lang/String;)V".equals(candidate.desc))
                    continue;

                FieldInsnNode loadedField = findPreviousScreenField(candidate, type.name, widgetOwner, null, 20);

                if (loadedField == null || !textField.name.equals(loadedField.name))
                    continue;

                if (result != null && !result.name.equals(candidate.name))
                    return null;

                result = candidate;
            }
        }

        return result;
    }

    private static boolean containsEquivalent(List<DirectConnectPlan> plans, DirectConnectPlan candidate) {
        for (DirectConnectPlan plan : plans) {
            if (plan.describe().equals(candidate.describe()))
                return true;
        }

        return false;
    }

    private static boolean instrumentRenderThreadEntries(ClassNode type) {
        boolean changed = false;

        for (MethodNode method : type.methods) {
            if ("<init>".equals(method.name) || "<clinit>".equals(method.name)
                || (method.access & (Opcodes.ACC_STATIC | Opcodes.ACC_ABSTRACT | Opcodes.ACC_NATIVE)) != 0
                || alreadyInstrumented(method))
                continue;

            InsnList apply = new InsnList();
            apply.add(new VarInsnNode(Opcodes.ALOAD, 0));
            apply.add(new MethodInsnNode(Opcodes.INVOKESTATIC, ControllerName, "applyPending", "(Ljava/lang/Object;)V", false));
            method.instructions.insert(apply);
            changed = true;
        }

        return changed;
    }

    private static boolean alreadyInstrumented(MethodNode method) {
        for (AbstractInsnNode instruction = method.instructions.getFirst(); instruction != null; instruction = instruction.getNext()) {
            if (instruction instanceof MethodInsnNode) {
                MethodInsnNode call = (MethodInsnNode) instruction;

                if (ControllerName.equals(call.owner) && "applyPending".equals(call.name))
                    return true;
            }
        }

        return false;
    }
}
