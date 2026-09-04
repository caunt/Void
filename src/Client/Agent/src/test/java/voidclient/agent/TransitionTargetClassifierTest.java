package voidclient.agent;

import org.junit.Assert;
import org.junit.Test;
import org.objectweb.asm.Opcodes;
import org.objectweb.asm.tree.FieldInsnNode;
import org.objectweb.asm.tree.InsnNode;
import org.objectweb.asm.tree.MethodNode;
import org.objectweb.asm.tree.TypeInsnNode;
import org.objectweb.asm.tree.VarInsnNode;

public final class TransitionTargetClassifierTest {
    @Test
    public void classifiesNullAsGameTarget() {
        Assert.assertEquals(GameAutomationIndex.TransitionTargetKind.GAME,
            classify(instanceMethod(), new InsnNode(Opcodes.ACONST_NULL), null));
    }

    @Test
    public void classifiesInstanceReceiverAsSelfTarget() {
        Assert.assertEquals(GameAutomationIndex.TransitionTargetKind.SELF,
            classify(instanceMethod(), new VarInsnNode(Opcodes.ALOAD, 0), null));
    }

    @Test
    public void doesNotClassifyStaticArgumentAsSelfTarget() {
        Assert.assertEquals(GameAutomationIndex.TransitionTargetKind.PARENT,
            classify(staticMethod(), new VarInsnNode(Opcodes.ALOAD, 0), null));
    }

    @Test
    public void classifiesConstructedScreenAsConstructedTarget() {
        Assert.assertEquals(GameAutomationIndex.TransitionTargetKind.CONSTRUCTED,
            classify(instanceMethod(), new InsnNode(Opcodes.DUP), new TypeInsnNode(Opcodes.NEW, "synthetic/Screen")));
    }

    @Test
    public void classifiesScreenFieldAsParentTarget() {
        Assert.assertEquals(GameAutomationIndex.TransitionTargetKind.PARENT,
            classify(instanceMethod(), new FieldInsnNode(Opcodes.GETFIELD, "synthetic/Screen", "parent", "Lsynthetic/Screen;"), null));
    }

    private static GameAutomationIndex.TransitionTargetKind classify(MethodNode method, org.objectweb.asm.tree.AbstractInsnNode argument,
                                                                      TypeInsnNode constructed) {
        return GameAutomationIndex.classifyTransitionTarget(method, argument, constructed);
    }

    private static MethodNode instanceMethod() {
        return new MethodNode(Opcodes.ACC_PUBLIC, "transition", "()V", null, null);
    }

    private static MethodNode staticMethod() {
        return new MethodNode(Opcodes.ACC_PUBLIC | Opcodes.ACC_STATIC, "transition", "(Ljava/lang/Object;)V", null, null);
    }
}
