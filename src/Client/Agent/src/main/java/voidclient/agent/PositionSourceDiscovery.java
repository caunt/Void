package voidclient.agent;

import org.objectweb.asm.ClassReader;
import org.objectweb.asm.ClassVisitor;
import org.objectweb.asm.MethodVisitor;
import org.objectweb.asm.Opcodes;
import org.objectweb.asm.tree.AbstractInsnNode;
import org.objectweb.asm.tree.ClassNode;
import org.objectweb.asm.tree.FieldInsnNode;
import org.objectweb.asm.tree.MethodInsnNode;
import org.objectweb.asm.tree.MethodNode;
import org.objectweb.asm.tree.VarInsnNode;

final class PositionSourceDiscovery {

    static PositionSource[] findPositionSources(byte[] classFileBuffer) {
        final PositionSource[][] result = new PositionSource[1][];

        new ClassReader(classFileBuffer).accept(new ClassVisitor(Opcodes.ASM9) {
            @Override
            public MethodVisitor visitMethod(int access, String name, String descriptor, String signature, String[] exceptions) {
                if (result[0] != null || !"(DDD)D".equals(descriptor))
                    return null;

                return new MethodNode(Opcodes.ASM9, access, name, descriptor, signature, exceptions) {
                    @Override
                    public void visitEnd() {
                        result[0] = findPositionSources(this);
                    }
                };
            }
        }, ClassReader.SKIP_DEBUG | ClassReader.SKIP_FRAMES);

        return result[0];
    }

    static PositionSource[] findPositionSources(ClassNode classNode) {
        for (MethodNode method : classNode.methods) {
            PositionSource[] sources = findPositionSources(method);

            if (sources != null)
                return sources;
        }

        return null;
    }

    private static PositionSource[] findPositionSources(MethodNode method) {
        if (!"(DDD)D".equals(method.desc))
            return null;

        PositionSource[] sources = new PositionSource[3];
        int multiplyCount = 0;
        int addCount = 0;

        for (AbstractInsnNode instruction = method.instructions.getFirst(); instruction != null; instruction = instruction.getNext()) {
            if (instruction.getOpcode() == Opcodes.DMUL)
                multiplyCount++;
            else if (instruction.getOpcode() == Opcodes.DADD)
                addCount++;

            if (!(instruction instanceof VarInsnNode) || instruction.getOpcode() != Opcodes.DLOAD)
                continue;

            int variable = ((VarInsnNode) instruction).var;
            int coordinateIndex = variable == 1 ? 0 : variable == 3 ? 1 : variable == 5 ? 2 : -1;

            if (coordinateIndex < 0 || sources[coordinateIndex] != null)
                continue;

            AbstractInsnNode previous = previousMeaningful(instruction.getPrevious());

            if (previous instanceof FieldInsnNode && previous.getOpcode() == Opcodes.GETFIELD && "D".equals(((FieldInsnNode) previous).desc)) {
                FieldInsnNode field = (FieldInsnNode) previous;
                sources[coordinateIndex] = PositionSource.field(field.owner, field.name);
            } else if (previous instanceof MethodInsnNode && previous.getOpcode() != Opcodes.INVOKESTATIC && "()D".equals(((MethodInsnNode) previous).desc)) {
                MethodInsnNode invokedMethod = (MethodInsnNode) previous;
                sources[coordinateIndex] = PositionSource.method(invokedMethod.owner, invokedMethod.name);
            }
        }

        return sources[0] != null && sources[1] != null && sources[2] != null && multiplyCount >= 3 && addCount >= 2 ? sources : null;
    }

    private static AbstractInsnNode previousMeaningful(AbstractInsnNode instruction) {
        while (instruction != null && (instruction.getType() == AbstractInsnNode.LABEL || instruction.getType() == AbstractInsnNode.LINE || instruction.getType() == AbstractInsnNode.FRAME))
            instruction = instruction.getPrevious();

        return instruction;
    }
}
