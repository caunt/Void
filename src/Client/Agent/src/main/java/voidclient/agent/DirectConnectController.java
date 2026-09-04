package voidclient.agent;

import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.TimeUnit;

public final class DirectConnectController {
    private static final long RequestTimeoutMilliseconds = 5000;
    private static final Map<String, DirectConnectPlan> Plans = new HashMap<String, DirectConnectPlan>();
    private static final Map<String, String> DiscoveryFailures = new HashMap<String, String>();
    private static final ThreadLocal<Boolean> Applying = new ThreadLocal<Boolean>();
    private static PendingRequest pendingRequest;

    private DirectConnectController() {
    }

    static synchronized void registerPlan(DirectConnectPlan plan) {
        Plans.put(plan.screenClassName, plan);
        DiscoveryFailures.remove(plan.screenClassName);
        System.err.println("Void client agent discovered Direct Connect: " + plan.describe());
    }

    static synchronized void recordDiscoveryFailure(String className, String message) {
        DiscoveryFailures.put(className, message);
        System.err.println("Void client agent could not instrument Direct Connect candidate " + className + ": " + message);
    }

    static String connectJson(String address) {
        PendingRequest request = new PendingRequest(address);

        synchronized (DirectConnectController.class) {
            if (Plans.isEmpty())
                return error("screen.discovery", "No compatible Direct Connect screen has been discovered; " + discoveryDetails());

            if (pendingRequest != null)
                return error("request.pending", "Another Direct Connect request is already pending");

            pendingRequest = request;
        }

        try {
            if (!request.completed.await(RequestTimeoutMilliseconds, TimeUnit.MILLISECONDS)) {
                synchronized (DirectConnectController.class) {
                    if (pendingRequest == request)
                        pendingRequest = null;
                }

                return error("screen.activation", "The discovered Direct Connect screen did not execute on the render thread within 5.0 seconds; " + discoveryDetails());
            }
        } catch (InterruptedException exception) {
            Thread.currentThread().interrupt();
            return error("request.interrupted", "The Direct Connect request was interrupted");
        }

        return request.response;
    }

    public static void applyPending(Object screen) {
        if (screen == null || Boolean.TRUE.equals(Applying.get()))
            return;

        PendingRequest request;
        DirectConnectPlan plan;

        synchronized (DirectConnectController.class) {
            request = pendingRequest;

            if (request == null)
                return;

            plan = Plans.get(screen.getClass().getName().replace('.', '/'));

            if (plan == null)
                return;

            pendingRequest = null;
        }

        Applying.set(Boolean.TRUE);

        try {
            String value = connect(screen, plan, request.address);

            if (!request.address.equals(value))
                request.response = error("address.verify", "Minecraft returned " + quote(value) + " instead of " + quote(request.address) + "; " + plan.describe());
            else
                request.response = "{\"status\":\"ok\",\"stage\":\"connection.submit\",\"value\":" + quote(value) + "}";
        } catch (Throwable exception) {
            request.response = error("connection.submit", exception.getClass().getName() + ": " + exception.getMessage() + "; " + plan.describe());
        } finally {
            Applying.remove();
            request.completed.countDown();
        }
    }

    private static String connect(Object screen, DirectConnectPlan plan, String address) throws Exception {
        ClassLoader loader = screen.getClass().getClassLoader();
        Field textField = declaredField(screen.getClass(), plan.textFieldName);
        Object textWidget = textField.get(screen);
        Class<?> widgetOwner = loadClass(loader, plan.setterOwner);
        Method setter = declaredMethod(widgetOwner, plan.setterName, String.class);
        setter.invoke(textWidget, address);

        Method getter = declaredMethod(loadClass(loader, plan.getterOwner), plan.getterName);
        String exactValue = (String) getter.invoke(textWidget);

        Field serverDataField = declaredField(screen.getClass(), plan.serverDataFieldName);
        Object serverData = serverDataField.get(screen);
        Field addressField = declaredField(loadClass(loader, plan.addressOwner), plan.addressFieldName);
        addressField.set(serverData, exactValue);

        Field callbackField = declaredField(screen.getClass(), plan.callbackFieldName);
        Object callback = callbackField.get(screen);
        Method callbackMethod;

        if ("(Z)V".equals(plan.callbackDescriptor)) {
            callbackMethod = declaredMethod(loadClass(loader, plan.callbackOwner), plan.callbackName, Boolean.TYPE);
            callbackMethod.invoke(callback, Boolean.TRUE);
        } else if ("(ZI)V".equals(plan.callbackDescriptor)) {
            callbackMethod = declaredMethod(loadClass(loader, plan.callbackOwner), plan.callbackName, Boolean.TYPE, Integer.TYPE);
            callbackMethod.invoke(callback, Boolean.TRUE, Integer.valueOf(0));
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

    private static Method declaredMethod(Class<?> owner, String name, Class<?>... parameterTypes) throws NoSuchMethodException {
        Method method = owner.getDeclaredMethod(name, parameterTypes);
        method.setAccessible(true);
        return method;
    }

    private static synchronized String discoveryDetails() {
        return "plans=" + Plans.size() + ", failures=" + DiscoveryFailures;
    }

    private static String error(String stage, String message) {
        return "{\"status\":\"unavailable\",\"stage\":" + quote(stage) + ",\"message\":" + quote(message) + "}";
    }

    private static String quote(String value) {
        if (value == null)
            return "null";

        StringBuilder result = new StringBuilder(value.length() + 2);
        result.append('"');

        for (int index = 0; index < value.length(); index++) {
            char character = value.charAt(index);

            if (character == '"' || character == '\\')
                result.append('\\').append(character);
            else if (character == '\n')
                result.append("\\n");
            else if (character == '\r')
                result.append("\\r");
            else if (character == '\t')
                result.append("\\t");
            else if (character < 0x20)
                result.append(String.format("\\u%04x", Integer.valueOf(character)));
            else
                result.append(character);
        }

        return result.append('"').toString();
    }

    private static final class PendingRequest {
        private final String address;
        private final CountDownLatch completed = new CountDownLatch(1);
        private String response;

        private PendingRequest(String address) {
            this.address = address;
        }
    }
}
