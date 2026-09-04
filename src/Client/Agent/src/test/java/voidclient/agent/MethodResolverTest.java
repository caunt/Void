package voidclient.agent;

import java.lang.reflect.Method;
import org.junit.Assert;
import org.junit.Test;

public final class MethodResolverTest {
    @Test
    public void resolvesMethodDeclaredByOwner() throws Exception {
        Method method = MethodResolver.resolve(Child.class, "declared");

        Assert.assertEquals(Child.class, method.getDeclaringClass());
        Assert.assertEquals("child", method.invoke(new Child()));
    }

    @Test
    public void resolvesInheritedPublicMethod() throws Exception {
        Method method = MethodResolver.resolve(Child.class, "publicParent", String.class);

        Assert.assertEquals(Parent.class, method.getDeclaringClass());
        Assert.assertEquals("value", method.invoke(new Child(), "value"));
    }

    @Test
    public void resolvesInheritedNonPublicMethod() throws Exception {
        Method method = MethodResolver.resolve(Child.class, "protectedParent");

        Assert.assertEquals(Parent.class, method.getDeclaringClass());
        Assert.assertEquals("protected", method.invoke(new Child()));
    }

    @Test
    public void resolvesInheritedInterfaceMethod() throws Exception {
        Method method = MethodResolver.resolve(ChildInterface.class, "interfaceMethod");

        Assert.assertEquals(ParentInterface.class, method.getDeclaringClass());
        Assert.assertEquals("interface", method.invoke(new InterfaceImplementation()));
    }

    @Test
    public void prefersChildOverride() throws Exception {
        Method method = MethodResolver.resolve(OverridingChild.class, "publicParent", String.class);

        Assert.assertEquals(OverridingChild.class, method.getDeclaringClass());
        Assert.assertEquals("overridden", method.invoke(new OverridingChild(), "value"));
    }

    @Test
    public void reportsOriginalLookupWhenMethodIsMissing() {
        try {
            MethodResolver.resolve(Child.class, "missing", String.class, Integer.TYPE);
            Assert.fail("Expected method resolution to fail");
        } catch (NoSuchMethodException exception) {
            Assert.assertEquals(Child.class.getName() + ".missing(java.lang.String,int)", exception.getMessage());
        }
    }

    private static class Parent {
        public String publicParent(String value) {
            return value;
        }

        protected String protectedParent() {
            return "protected";
        }
    }

    private static class Child extends Parent {
        public String declared() {
            return "child";
        }
    }

    private static final class OverridingChild extends Parent {
        @Override
        public String publicParent(String value) {
            return "overridden";
        }
    }

    private interface ParentInterface {
        default String interfaceMethod() {
            return "interface";
        }
    }

    private interface ChildInterface extends ParentInterface {
    }

    private static final class InterfaceImplementation implements ChildInterface {
    }
}
