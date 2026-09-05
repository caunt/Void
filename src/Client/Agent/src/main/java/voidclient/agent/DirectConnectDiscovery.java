package voidclient.agent;

import java.util.ArrayList;
import java.util.List;
import org.objectweb.asm.Opcodes;
import org.objectweb.asm.Type;
import org.objectweb.asm.tree.AbstractInsnNode;
import org.objectweb.asm.tree.ClassNode;
import org.objectweb.asm.tree.FieldInsnNode;
import org.objectweb.asm.tree.IntInsnNode;
import org.objectweb.asm.tree.LdcInsnNode;
import org.objectweb.asm.tree.MethodInsnNode;
import org.objectweb.asm.tree.MethodNode;

final class DirectConnectDiscovery {

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

                Object[] callbackArguments = callbackArguments(callback);
                if (callbackArguments == null)
                    continue;

                DirectConnectPlan candidate = new DirectConnectPlan(type.name, textField.name, textField.desc,
                    setter.owner, setter.name, getter.owner, getter.name, serverDataField.name, serverDataField.desc,
                    addressField.owner, addressField.name, callbackField.name, callbackField.desc,
                    callback.owner, callback.name, callback.desc, callbackArguments);

                if (!containsEquivalent(candidates, candidate))
                    candidates.add(candidate);
            }
        }

        return candidates.size() == 1 ? candidates.get(0) : null;
    }

    static Object[] callbackArguments(MethodInsnNode callback) {
        Type[] types = Type.getArgumentTypes(callback.desc);
        Object[] values = new Object[types.length];
        AbstractInsnNode instruction = callback.getPrevious();
        for (int index = types.length - 1; index >= 0; index--) {
            while (instruction != null && instruction.getOpcode() < 0)
                instruction = instruction.getPrevious();
            if (instruction == null)
                return null;
            Integer constant = null;
            int opcode = instruction.getOpcode();
            if (opcode >= Opcodes.ICONST_M1 && opcode <= Opcodes.ICONST_5)
                constant = Integer.valueOf(opcode - Opcodes.ICONST_0);
            else if (instruction instanceof IntInsnNode)
                constant = Integer.valueOf(((IntInsnNode) instruction).operand);
            else if (instruction instanceof LdcInsnNode && ((LdcInsnNode) instruction).cst instanceof Integer)
                constant = (Integer) ((LdcInsnNode) instruction).cst;
            if (constant == null)
                return null;
            values[index] = types[index].getSort() == Type.BOOLEAN ? Boolean.valueOf(constant.intValue() != 0) : (Object) constant;
            instruction = instruction.getPrevious();
        }
        return values;
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

}
