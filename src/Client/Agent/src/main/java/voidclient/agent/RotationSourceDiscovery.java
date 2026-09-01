package voidclient.agent;

import java.util.ArrayList;
import java.util.List;
import org.objectweb.asm.ClassReader;
import org.objectweb.asm.Opcodes;
import org.objectweb.asm.tree.AbstractInsnNode;
import org.objectweb.asm.tree.ClassNode;
import org.objectweb.asm.tree.FieldInsnNode;
import org.objectweb.asm.tree.FieldNode;
import org.objectweb.asm.tree.MethodInsnNode;
import org.objectweb.asm.tree.MethodNode;
import org.objectweb.asm.tree.VarInsnNode;

final class RotationSourceDiscovery {

    static RotationSources findRotationSources(byte[] classFileBuffer) {
        ClassNode type = new ClassNode();
        new ClassReader(classFileBuffer).accept(type, ClassReader.SKIP_DEBUG | ClassReader.SKIP_FRAMES);
        return findRotationSources(type);
    }

    static RotationSources findRotationSources(ClassNode type) {
        PositionSource[] bodyAndHead = findBodyAndHeadYaw(type);
        PositionSource pitch = findPitch(type);

        if (bodyAndHead == null && pitch == null)
            return null;

        return new RotationSources(bodyAndHead == null ? null : bodyAndHead[0], bodyAndHead == null ? null : bodyAndHead[1], pitch);
    }

    private static PositionSource[] findBodyAndHeadYaw(ClassNode type) {
        PositionSource body = findBodyYaw(type);

        if (body == null)
            return null;

        PositionSource head = findHeadYaw(type, body);
        return head == null ? null : new PositionSource[] { body, head };
    }

    private static PositionSource findBodyYaw(ClassNode type) {
        for (MethodNode method : type.methods) {
            if (!method.desc.startsWith("(F"))
                continue;

            for (AbstractInsnNode instruction = method.instructions.getFirst(); instruction != null; instruction = instruction.getNext()) {
                if (instruction.getOpcode() != Opcodes.FSUB)
                    continue;

                AbstractInsnNode sourceInstruction = previousMeaningful(instruction.getPrevious());
                PositionSource source = readSource(sourceInstruction);

                if (source == null || source.method || !isWritten(method, source))
                    continue;

                AbstractInsnNode receiver = previousMeaningful(sourceInstruction.getPrevious());
                AbstractInsnNode parameter = receiver == null ? null : previousMeaningful(receiver.getPrevious());

                if (receiver instanceof VarInsnNode && receiver.getOpcode() == Opcodes.ALOAD && parameter instanceof VarInsnNode && parameter.getOpcode() == Opcodes.FLOAD && ((VarInsnNode) parameter).var == 1)
                    return source;
            }
        }

        return null;
    }

    private static PositionSource findHeadYaw(ClassNode type, PositionSource body) {
        PositionSource best = null;
        int bestFieldDistance = Integer.MAX_VALUE;
        int bestDistance = Integer.MAX_VALUE;
        int bodyFieldIndex = fieldIndex(type, body);

        for (MethodNode method : type.methods) {
            List<FieldCopy> copies = new ArrayList<FieldCopy>();
            int instructionIndex = 0;

            for (AbstractInsnNode instruction = method.instructions.getFirst(); instruction != null; instruction = instruction.getNext()) {
                instructionIndex++;

                if (!(instruction instanceof FieldInsnNode) || instruction.getOpcode() != Opcodes.PUTFIELD || !"F".equals(((FieldInsnNode) instruction).desc))
                    continue;

                AbstractInsnNode sourceInstruction = previousMeaningful(instruction.getPrevious());

                if (!(sourceInstruction instanceof FieldInsnNode) || sourceInstruction.getOpcode() != Opcodes.GETFIELD || !"F".equals(((FieldInsnNode) sourceInstruction).desc))
                    continue;

                FieldInsnNode sourceField = (FieldInsnNode) sourceInstruction;
                FieldInsnNode targetField = (FieldInsnNode) instruction;
                PositionSource source = PositionSource.field(sourceField.owner, sourceField.name);
                PositionSource target = PositionSource.field(targetField.owner, targetField.name);

                if (!sameSource(source, target))
                    copies.add(new FieldCopy(source, target, instructionIndex));
            }

            for (FieldCopy bodyCopy : copies) {
                if (!sameSource(bodyCopy.source, body))
                    continue;

                for (FieldCopy candidate : copies) {
                    if (sameSource(candidate.source, body) || !candidate.source.owner.equals(body.owner))
                        continue;

                    int distance = Math.abs(candidate.instructionIndex - bodyCopy.instructionIndex);
                    int candidateFieldIndex = fieldIndex(type, candidate.source);
                    int fieldDistance = bodyFieldIndex < 0 || candidateFieldIndex < 0 ? Integer.MAX_VALUE : Math.abs(candidateFieldIndex - bodyFieldIndex);

                    if (distance <= 8 && (fieldDistance < bestFieldDistance || fieldDistance == bestFieldDistance && distance < bestDistance)) {
                        best = candidate.source;
                        bestFieldDistance = fieldDistance;
                        bestDistance = distance;
                    }
                }
            }
        }

        return best;
    }

    private static int fieldIndex(ClassNode type, PositionSource source) {
        if (!type.name.equals(source.owner))
            return -1;

        for (int index = 0; index < type.fields.size(); index++) {
            FieldNode field = type.fields.get(index);

            if (source.name.equals(field.name) && "F".equals(field.desc))
                return index;
        }

        return -1;
    }

    private static PositionSource findPitch(ClassNode type) {
        for (MethodNode method : type.methods) {
            if (!"(FF)V".equals(method.desc) || countOpcode(method, Opcodes.FREM) < 2)
                continue;

            PositionSource yaw = findParameterTarget(type, method, 1);
            PositionSource pitch = findParameterTarget(type, method, 2);

            if (yaw != null && pitch != null && !sameSource(yaw, pitch))
                return pitch;
        }

        return null;
    }

    private static PositionSource findParameterTarget(ClassNode type, MethodNode method, int parameter) {
        for (AbstractInsnNode instruction = method.instructions.getFirst(); instruction != null; instruction = instruction.getNext()) {
            PositionSource target = writeTarget(type, instruction);

            if (target == null)
                continue;

            boolean hasRemainder = false;

            int inspected = 0;

            for (AbstractInsnNode previous = previousMeaningful(instruction.getPrevious()); previous != null && inspected++ < 8; previous = previousMeaningful(previous.getPrevious())) {
                if (previous instanceof VarInsnNode && previous.getOpcode() == Opcodes.FLOAD) {
                    int variable = ((VarInsnNode) previous).var;

                    if (variable == parameter && hasRemainder)
                        return target;

                    break;
                }

                if (previous.getOpcode() == Opcodes.FREM)
                    hasRemainder = true;

                if (previous.getOpcode() == Opcodes.PUTFIELD || previous instanceof MethodInsnNode)
                    break;
            }
        }

        return null;
    }

    private static PositionSource writeTarget(ClassNode type, AbstractInsnNode instruction) {
        if (instruction instanceof FieldInsnNode && instruction.getOpcode() == Opcodes.PUTFIELD && "F".equals(((FieldInsnNode) instruction).desc)) {
            FieldInsnNode field = (FieldInsnNode) instruction;
            return PositionSource.field(field.owner, field.name);
        }

        if (!(instruction instanceof MethodInsnNode) || instruction.getOpcode() == Opcodes.INVOKESTATIC || !"(F)V".equals(((MethodInsnNode) instruction).desc))
            return null;

        MethodInsnNode invocation = (MethodInsnNode) instruction;

        if (!type.name.equals(invocation.owner))
            return null;

        for (MethodNode method : type.methods) {
            if (!invocation.name.equals(method.name) || !invocation.desc.equals(method.desc))
                continue;

            for (AbstractInsnNode candidate = method.instructions.getFirst(); candidate != null; candidate = candidate.getNext()) {
                if (candidate instanceof FieldInsnNode && candidate.getOpcode() == Opcodes.PUTFIELD && "F".equals(((FieldInsnNode) candidate).desc)) {
                    FieldInsnNode field = (FieldInsnNode) candidate;
                    return PositionSource.field(field.owner, field.name);
                }
            }
        }

        return null;
    }

    private static PositionSource readSource(AbstractInsnNode instruction) {
        if (instruction instanceof FieldInsnNode && instruction.getOpcode() == Opcodes.GETFIELD && "F".equals(((FieldInsnNode) instruction).desc)) {
            FieldInsnNode field = (FieldInsnNode) instruction;
            return PositionSource.field(field.owner, field.name);
        }

        if (instruction instanceof MethodInsnNode && instruction.getOpcode() != Opcodes.INVOKESTATIC && "()F".equals(((MethodInsnNode) instruction).desc)) {
            MethodInsnNode method = (MethodInsnNode) instruction;
            return PositionSource.method(method.owner, method.name);
        }

        return null;
    }

    private static boolean isWritten(MethodNode method, PositionSource source) {
        if (source.method)
            return false;

        for (AbstractInsnNode instruction = method.instructions.getFirst(); instruction != null; instruction = instruction.getNext()) {
            if (!(instruction instanceof FieldInsnNode) || instruction.getOpcode() != Opcodes.PUTFIELD)
                continue;

            FieldInsnNode field = (FieldInsnNode) instruction;

            if (source.owner.equals(field.owner) && source.name.equals(field.name) && "F".equals(field.desc))
                return true;
        }

        return false;
    }

    private static int countOpcode(MethodNode method, int opcode) {
        int count = 0;

        for (AbstractInsnNode instruction = method.instructions.getFirst(); instruction != null; instruction = instruction.getNext()) {
            if (instruction.getOpcode() == opcode)
                count++;
        }

        return count;
    }

    private static boolean sameSource(PositionSource left, PositionSource right) {
        return left.method == right.method && left.owner.equals(right.owner) && left.name.equals(right.name);
    }

    private static AbstractInsnNode previousMeaningful(AbstractInsnNode instruction) {
        while (instruction != null && (instruction.getType() == AbstractInsnNode.LABEL || instruction.getType() == AbstractInsnNode.LINE || instruction.getType() == AbstractInsnNode.FRAME))
            instruction = instruction.getPrevious();

        return instruction;
    }

    private static final class FieldCopy {
        private final PositionSource source;
        private final PositionSource target;
        private final int instructionIndex;

        private FieldCopy(PositionSource source, PositionSource target, int instructionIndex) {
            this.source = source;
            this.target = target;
            this.instructionIndex = instructionIndex;
        }
    }
}
