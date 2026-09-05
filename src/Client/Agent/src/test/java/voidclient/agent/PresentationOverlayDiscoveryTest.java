package voidclient.agent;

import java.util.HashMap;
import java.util.Map;
import org.junit.Assert;
import org.junit.Test;
import org.objectweb.asm.Opcodes;
import org.objectweb.asm.tree.ClassNode;
import org.objectweb.asm.tree.FieldInsnNode;
import org.objectweb.asm.tree.FieldNode;
import org.objectweb.asm.tree.InsnNode;
import org.objectweb.asm.tree.JumpInsnNode;
import org.objectweb.asm.tree.LabelNode;
import org.objectweb.asm.tree.MethodNode;
import org.objectweb.asm.tree.VarInsnNode;

public final class PresentationOverlayDiscoveryTest {
    @Test
    public void discoversPresentationOverlayFromSharedInterfaceAndControlFlow() {
        Fixture fixture = fixture(true, false, true);

        PresentationOverlayPlan result = GameAutomationIndex.discoverPresentationOverlay(
            fixture.types, fixture.superTypes, fixture.client.name, fixture.screen.name);

        Assert.assertNotNull(result);
        Assert.assertEquals(fixture.client.name, result.owner);
        Assert.assertEquals("presentation", result.fieldName);
        Assert.assertEquals("Lsynthetic/Presentation;", result.fieldDescriptor);
    }

    @Test
    public void leavesLegacyLayoutWithoutOverlayUnchanged() {
        Fixture fixture = fixture(false, false, true);

        Assert.assertNull(GameAutomationIndex.discoverPresentationOverlay(
            fixture.types, fixture.superTypes, fixture.client.name, fixture.screen.name));
    }

    @Test
    public void ignoresPresentationFieldWithoutRepeatedScreenControlFlow() {
        Fixture fixture = fixture(true, false, false);

        Assert.assertNull(GameAutomationIndex.discoverPresentationOverlay(
            fixture.types, fixture.superTypes, fixture.client.name, fixture.screen.name));
    }

    @Test
    public void rejectsAmbiguousPresentationFields() {
        Fixture fixture = fixture(true, true, true);

        try {
            GameAutomationIndex.discoverPresentationOverlay(
                fixture.types, fixture.superTypes, fixture.client.name, fixture.screen.name);
            Assert.fail("Expected ambiguous overlay discovery to fail");
        } catch (IllegalStateException exception) {
            Assert.assertTrue(exception.getMessage().contains("Expected at most one structural presentation overlay"));
        }
    }

    @Test
    public void readsActiveOverlayWithoutVersionSpecificTypes() throws Exception {
        RuntimeClient client = new RuntimeClient();
        PresentationOverlayPlan plan = new PresentationOverlayPlan(
            RuntimeClient.class.getName().replace('.', '/'), "presentation", "Ljava/lang/Object;");

        Assert.assertFalse(GameAutomationController.isPresentationOverlayActive(client, plan));

        client.presentation = new Object();

        Assert.assertTrue(GameAutomationController.isPresentationOverlayActive(client, plan));
    }

    private static Fixture fixture(boolean includePresentation, boolean includeSecondPresentation, boolean includeControlFlow) {
        ClassNode renderable = type("synthetic/Renderable", "java/lang/Object", Opcodes.ACC_PUBLIC | Opcodes.ACC_INTERFACE | Opcodes.ACC_ABSTRACT);
        ClassNode screen = type("synthetic/Screen", "java/lang/Object", Opcodes.ACC_PUBLIC | Opcodes.ACC_ABSTRACT);
        screen.interfaces.add(renderable.name);
        ClassNode presentation = type("synthetic/Presentation", "java/lang/Object", Opcodes.ACC_PUBLIC | Opcodes.ACC_ABSTRACT);
        presentation.interfaces.add(renderable.name);
        ClassNode secondPresentation = type("synthetic/SecondPresentation", "java/lang/Object", Opcodes.ACC_PUBLIC | Opcodes.ACC_ABSTRACT);
        secondPresentation.interfaces.add(renderable.name);
        ClassNode client = type("synthetic/Client", "java/lang/Object", Opcodes.ACC_PUBLIC);
        FieldNode screenField = field(client, "screen", "Lsynthetic/Screen;");

        if (includePresentation) {
            FieldNode presentationField = field(client, "presentation", "Lsynthetic/Presentation;");
            addSetter(client, presentationField);

            if (includeControlFlow) {
                addGuard(client, "firstGuard", screenField, presentationField);
                addGuard(client, "secondGuard", screenField, presentationField);
            }
        }

        if (includeSecondPresentation) {
            FieldNode secondField = field(client, "secondPresentation", "Lsynthetic/SecondPresentation;");
            addSetter(client, secondField);
            addGuard(client, "thirdGuard", screenField, secondField);
            addGuard(client, "fourthGuard", screenField, secondField);
        }

        Map<String, ClassNode> types = new HashMap<String, ClassNode>();
        types.put(renderable.name, renderable);
        types.put(screen.name, screen);
        types.put(presentation.name, presentation);
        types.put(secondPresentation.name, secondPresentation);
        types.put(client.name, client);
        Map<String, String> superTypes = new HashMap<String, String>();

        for (ClassNode type : types.values())
            superTypes.put(type.name, type.superName);

        return new Fixture(types, superTypes, client, screen);
    }

    private static ClassNode type(String name, String superName, int access) {
        ClassNode type = new ClassNode();
        type.name = name;
        type.superName = superName;
        type.access = access;
        return type;
    }

    private static FieldNode field(ClassNode owner, String name, String descriptor) {
        FieldNode field = new FieldNode(Opcodes.ACC_PUBLIC, name, descriptor, null, null);
        owner.fields.add(field);
        return field;
    }

    private static void addSetter(ClassNode owner, FieldNode field) {
        MethodNode method = new MethodNode(Opcodes.ACC_PUBLIC, "set" + field.name, '(' + field.desc + ")V", null, null);
        method.instructions.add(new VarInsnNode(Opcodes.ALOAD, 0));
        method.instructions.add(new VarInsnNode(Opcodes.ALOAD, 1));
        method.instructions.add(new FieldInsnNode(Opcodes.PUTFIELD, owner.name, field.name, field.desc));
        method.instructions.add(new InsnNode(Opcodes.RETURN));
        owner.methods.add(method);
    }

    private static void addGuard(ClassNode owner, String name, FieldNode screen, FieldNode presentation) {
        MethodNode method = new MethodNode(Opcodes.ACC_PUBLIC, name, "()V", null, null);
        LabelNode end = new LabelNode();
        method.instructions.add(new VarInsnNode(Opcodes.ALOAD, 0));
        method.instructions.add(new FieldInsnNode(Opcodes.GETFIELD, owner.name, screen.name, screen.desc));
        method.instructions.add(new JumpInsnNode(Opcodes.IFNULL, end));
        method.instructions.add(new VarInsnNode(Opcodes.ALOAD, 0));
        method.instructions.add(new FieldInsnNode(Opcodes.GETFIELD, owner.name, presentation.name, presentation.desc));
        method.instructions.add(new JumpInsnNode(Opcodes.IFNONNULL, end));
        method.instructions.add(end);
        method.instructions.add(new InsnNode(Opcodes.RETURN));
        owner.methods.add(method);
    }

    private static final class RuntimeClient {
        private Object presentation;
    }

    private static final class Fixture {
        final Map<String, ClassNode> types;
        final Map<String, String> superTypes;
        final ClassNode client;
        final ClassNode screen;

        Fixture(Map<String, ClassNode> types, Map<String, String> superTypes, ClassNode client, ClassNode screen) {
            this.types = types;
            this.superTypes = superTypes;
            this.client = client;
            this.screen = screen;
        }
    }
}
