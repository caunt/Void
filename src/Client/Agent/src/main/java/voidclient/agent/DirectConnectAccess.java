package voidclient.agent;

import java.lang.reflect.Field;
import java.lang.reflect.Method;

final class DirectConnectAccess {
    private DirectConnectAccess() {
    }

    static String connect(Object screen, DirectConnectPlan plan, String address) throws Exception {
        ClassLoader loader = screen.getClass().getClassLoader();
        Field textField = declaredField(screen.getClass(), plan.textFieldName);
        Object textWidget = textField.get(screen);
        Method setter = MethodResolver.resolve(loadClass(loader, plan.setterOwner), plan.setterName, String.class);
        setter.invoke(textWidget, address);
        Method getter = MethodResolver.resolve(loadClass(loader, plan.getterOwner), plan.getterName);
        String exactValue = (String) getter.invoke(textWidget);
        Field serverDataField = declaredField(screen.getClass(), plan.serverDataFieldName);
        Object serverData = serverDataField.get(screen);
        Field addressField = declaredField(loadClass(loader, plan.addressOwner), plan.addressFieldName);
        addressField.set(serverData, exactValue);
        Field callbackField = declaredField(screen.getClass(), plan.callbackFieldName);
        Object callback = callbackField.get(screen);
        Method callbackMethod;

        if ("(Z)V".equals(plan.callbackDescriptor)) {
            callbackMethod = MethodResolver.resolve(loadClass(loader, plan.callbackOwner), plan.callbackName, Boolean.TYPE);
            callbackMethod.invoke(callback, plan.callbackArguments);
        } else if ("(ZI)V".equals(plan.callbackDescriptor)) {
            callbackMethod = MethodResolver.resolve(loadClass(loader, plan.callbackOwner), plan.callbackName, Boolean.TYPE, Integer.TYPE);
            callbackMethod.invoke(callback, plan.callbackArguments);
        } else {
            throw new IllegalStateException("Unsupported callback descriptor " + plan.callbackDescriptor);
        }

        return exactValue;
    }

    private static Class<?> loadClass(ClassLoader loader, String internalName) throws ClassNotFoundException {
        return Class.forName(internalName.replace('/', '.'), false, loader);
    }

    private static Field declaredField(Class<?> owner, String name) throws NoSuchFieldException {
        Field field = owner.getDeclaredField(name);
        field.setAccessible(true);
        return field;
    }
}
