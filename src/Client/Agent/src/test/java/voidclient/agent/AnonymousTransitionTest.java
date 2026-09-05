package voidclient.agent;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import org.junit.Assert;
import org.junit.Test;
import org.objectweb.asm.Opcodes;
import org.objectweb.asm.tree.ClassNode;
import org.objectweb.asm.tree.FieldNode;
import org.objectweb.asm.tree.InsnNode;
import org.objectweb.asm.tree.MethodInsnNode;
import org.objectweb.asm.tree.MethodNode;
import org.objectweb.asm.tree.TypeInsnNode;
import org.objectweb.asm.tree.VarInsnNode;

public final class AnonymousTransitionTest {
    @Test
    public void associatesAnonymousWidgetActionWithItsCapturedScreen() {
        Map<String, ClassNode> types = new HashMap<String, ClassNode>();
        Map<String, String> hierarchy = new HashMap<String, String>();
        ClassNode callback = new ClassNode();
        callback.name = "renamed/WidgetAction";
        callback.superName = "renamed/Widget";
        callback.fields.add(new FieldNode(Opcodes.ACC_FINAL | Opcodes.ACC_SYNTHETIC, "captured", "Lrenamed/Menu;", null, null));
        MethodNode action = new MethodNode(Opcodes.ACC_PUBLIC, "activate", "(DD)V", null, null);
        action.instructions.add(new TypeInsnNode(Opcodes.NEW, "renamed/Direct"));
        action.instructions.add(new MethodInsnNode(Opcodes.INVOKESPECIAL, "renamed/Direct", "<init>", "()V", false));
        action.instructions.add(new MethodInsnNode(Opcodes.INVOKEVIRTUAL, "renamed/Client", "set", "(Lrenamed/Screen;)V", false));
        action.instructions.add(new InsnNode(Opcodes.RETURN));
        callback.methods.add(action);
        types.put(callback.name, callback);
        hierarchy.put("renamed/Menu", "renamed/Screen");
        hierarchy.put("renamed/Direct", "renamed/Screen");
        hierarchy.put(callback.name, callback.superName);
        Map<String, List<TransitionPlan>> transitions = GameAutomationIndex.discoverTransitions(types, hierarchy,
            "renamed/Screen", new GameAutomationIndex.ScreenSetter("renamed/Client", "set", "(Lrenamed/Screen;)V"));
        TransitionPlan plan = transitions.get("renamed/Menu").get(0);
        Assert.assertEquals(callback.name, plan.owner);
        Assert.assertEquals("renamed/Direct", plan.targetScreenNames.get(0));
        Assert.assertArrayEquals(new boolean[] { false, false }, plan.usedArguments);
    }

    @Test
    public void distinguishesUnusedCoordinatesFromRequiredLiveArguments() {
        MethodNode method = new MethodNode(Opcodes.ACC_PUBLIC, "action", "(DLjava/lang/Object;J)V", null, null);
        method.instructions.add(new VarInsnNode(Opcodes.ALOAD, 3));
        method.instructions.add(new InsnNode(Opcodes.POP));
        Assert.assertArrayEquals(new boolean[] { false, true, false }, GameAutomationIndex.usedArguments(method));
    }
}
