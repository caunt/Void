package voidclient.agent;

import org.junit.Assert;
import org.junit.Test;
import org.objectweb.asm.Opcodes;
import org.objectweb.asm.tree.ClassNode;
import org.objectweb.asm.tree.FieldInsnNode;
import org.objectweb.asm.tree.FieldNode;
import org.objectweb.asm.tree.InsnNode;
import org.objectweb.asm.tree.MethodInsnNode;
import org.objectweb.asm.tree.MethodNode;
import org.objectweb.asm.tree.VarInsnNode;

public final class DirectConnectDiscoveryTest {
    @Test
    public void discoversModernBooleanCallbackWithoutMemberNames() {
        DirectConnectPlan plan = DirectConnectDiscovery.discover(createScreen("(Z)V"));

        Assert.assertNotNull(plan);
        Assert.assertEquals("synthetic/Screen", plan.screenClassName);
        Assert.assertEquals("text", plan.textFieldName);
        Assert.assertEquals("set", plan.setterName);
        Assert.assertEquals("get", plan.getterName);
        Assert.assertEquals("address", plan.addressFieldName);
        Assert.assertEquals("accept", plan.callbackName);
        Assert.assertEquals("(Z)V", plan.callbackDescriptor);
    }

    @Test
    public void discoversLegacyBooleanAndIntegerCallback() {
        DirectConnectPlan plan = DirectConnectDiscovery.discover(createScreen("(ZI)V"));

        Assert.assertNotNull(plan);
        Assert.assertEquals("(ZI)V", plan.callbackDescriptor);
    }

    @Test
    public void readsCallbackControlIdFromBytecodeRatherThanAssumingZero() {
        MethodNode method = new MethodNode();
        method.instructions.add(new InsnNode(Opcodes.ICONST_1));
        method.instructions.add(new org.objectweb.asm.tree.IntInsnNode(Opcodes.BIPUSH, 37));
        MethodInsnNode callback = new MethodInsnNode(Opcodes.INVOKEINTERFACE, "renamed/Callback", "apply", "(ZI)V", true);
        method.instructions.add(callback);
        Assert.assertArrayEquals(new Object[] { Boolean.TRUE, Integer.valueOf(37) }, DirectConnectDiscovery.callbackArguments(callback));
    }

    @Test
    public void rejectsScreenWithoutSubmitDataFlow() {
        ClassNode screen = createScreen("(Z)V");
        screen.methods.remove(screen.methods.size() - 1);

        Assert.assertNull(DirectConnectDiscovery.discover(screen));
    }

    @Test
    public void writesExactAddressAndInvokesCallbackOnce() throws Exception {
        final RuntimeScreen screen = new RuntimeScreen();
        String screenName = RuntimeScreen.class.getName().replace('.', '/');
        String widgetName = RuntimeWidget.class.getName().replace('.', '/');
        String serverDataName = RuntimeServerData.class.getName().replace('.', '/');
        String callbackName = RuntimeCallback.class.getName().replace('.', '/');
        DirectConnectPlan plan = new DirectConnectPlan(screenName,
            "text", "L" + widgetName + ";", widgetName, "set", widgetName, "get",
            "serverData", "L" + serverDataName + ";", serverDataName, "address",
            "callback", "L" + callbackName + ";", callbackName, "accept", "(ZI)V", new Object[] { Boolean.TRUE, Integer.valueOf(37) });

        String response = DirectConnectAccess.connect(screen, plan, "example.test:25565");
        Assert.assertEquals("example.test:25565", response);
        Assert.assertEquals("example.test:25565", screen.text.get());
        Assert.assertEquals("example.test:25565", screen.serverData.address);
        Assert.assertEquals(1, screen.callback.invocations);
        Assert.assertEquals(37, screen.callback.controlId);
    }

    private static ClassNode createScreen(String callbackDescriptor) {
        ClassNode screen = new ClassNode();
        screen.version = Opcodes.V1_8;
        screen.access = Opcodes.ACC_PUBLIC;
        screen.name = "synthetic/Screen";
        screen.superName = "java/lang/Object";
        screen.fields.add(new FieldNode(Opcodes.ACC_PRIVATE, "text", "Lsynthetic/Widget;", null, null));
        screen.fields.add(new FieldNode(Opcodes.ACC_PRIVATE, "serverData", "Lsynthetic/ServerData;", null, null));
        screen.fields.add(new FieldNode(Opcodes.ACC_PRIVATE, "callback", "Lsynthetic/Callback;", null, null));

        MethodNode initialize = new MethodNode(Opcodes.ACC_PUBLIC, "initialize", "()V", null, null);
        initialize.instructions.add(new VarInsnNode(Opcodes.ALOAD, 0));
        initialize.instructions.add(new FieldInsnNode(Opcodes.GETFIELD, screen.name, "text", "Lsynthetic/Widget;"));
        initialize.instructions.add(new VarInsnNode(Opcodes.ALOAD, 0));
        initialize.instructions.add(new FieldInsnNode(Opcodes.GETFIELD, screen.name, "serverData", "Lsynthetic/ServerData;"));
        initialize.instructions.add(new FieldInsnNode(Opcodes.GETFIELD, "synthetic/ServerData", "address", "Ljava/lang/String;"));
        initialize.instructions.add(new MethodInsnNode(Opcodes.INVOKEVIRTUAL, "synthetic/Widget", "set", "(Ljava/lang/String;)V", false));
        initialize.instructions.add(new InsnNode(Opcodes.RETURN));
        screen.methods.add(initialize);

        MethodNode submit = new MethodNode(Opcodes.ACC_PRIVATE, "submit", "()V", null, null);
        submit.instructions.add(new VarInsnNode(Opcodes.ALOAD, 0));
        submit.instructions.add(new FieldInsnNode(Opcodes.GETFIELD, screen.name, "serverData", "Lsynthetic/ServerData;"));
        submit.instructions.add(new VarInsnNode(Opcodes.ALOAD, 0));
        submit.instructions.add(new FieldInsnNode(Opcodes.GETFIELD, screen.name, "text", "Lsynthetic/Widget;"));
        submit.instructions.add(new MethodInsnNode(Opcodes.INVOKEVIRTUAL, "synthetic/Widget", "get", "()Ljava/lang/String;", false));
        submit.instructions.add(new FieldInsnNode(Opcodes.PUTFIELD, "synthetic/ServerData", "address", "Ljava/lang/String;"));
        submit.instructions.add(new VarInsnNode(Opcodes.ALOAD, 0));
        submit.instructions.add(new FieldInsnNode(Opcodes.GETFIELD, screen.name, "callback", "Lsynthetic/Callback;"));
        submit.instructions.add(new InsnNode(Opcodes.ICONST_1));

        if ("(ZI)V".equals(callbackDescriptor))
            submit.instructions.add(new InsnNode(Opcodes.ICONST_0));

        submit.instructions.add(new MethodInsnNode(Opcodes.INVOKEINTERFACE, "synthetic/Callback", "accept", callbackDescriptor, true));
        submit.instructions.add(new InsnNode(Opcodes.RETURN));
        screen.methods.add(submit);
        return screen;
    }

    public static final class RuntimeScreen {
        private final RuntimeWidget text = new RuntimeWidget();
        private final RuntimeServerData serverData = new RuntimeServerData();
        private final RuntimeCallback callback = new RuntimeCallback();
    }

    public static final class RuntimeWidget {
        private String value;

        public void set(String newValue) {
            value = newValue;
        }

        public String get() {
            return value;
        }
    }

    public static final class RuntimeServerData {
        private String address;
    }

    public static final class RuntimeCallback {
        private int invocations;
        private int controlId;

        public void accept(boolean accepted, int controlId) {
            this.controlId = controlId;
            if (accepted)
                invocations++;
        }
    }
}
