package voidclient.agent;

import java.lang.ref.WeakReference;
import java.lang.reflect.Array;
import java.lang.reflect.Constructor;
import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.lang.reflect.Modifier;
import java.lang.instrument.Instrumentation;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.HashMap;
import java.util.HashSet;
import java.util.IdentityHashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.Executor;
import org.objectweb.asm.Type;

public final class GameAutomationController {
    private static final Map<ClassLoader, GameAutomationIndex.IndexedCode> LoaderIndexes = Collections.synchronizedMap(new java.util.WeakHashMap<ClassLoader, GameAutomationIndex.IndexedCode>());
    private static final Map<String, String> IndexFailures = new HashMap<String, String>();
    private static final ThreadLocal<Boolean> Applying = new ThreadLocal<Boolean>();
    private static final ThreadLocal<Boolean> Dispatched = new ThreadLocal<Boolean>();
    private static volatile PendingOperation pending;
    private static WeakReference<Object> screenSetterReceiver = new WeakReference<Object>(null);
    private static Thread clientThread;
    private static volatile Object consumedScreen;
    private static String waitingScreenName;
    private static Instrumentation instrumentation;

    private GameAutomationController() {
    }

    static void initialize(Instrumentation value) {
        instrumentation = value;
    }

    static void registerIndex(ClassLoader loader, GameAutomationIndex.IndexedCode index) {
        if (index == null)
            return;

        if (LoaderIndexes.put(loader, index) == index)
            return;

        System.err.println("Void client agent indexed UI graph: direct=" + index.plan.directConnect.describe()
            + " chat=" + index.plan.chat.describe() + " screens=" + index.plan.transitions.size());
        retransformClientClass(index.plan.clientClassName);
        retransformClientClass(index.plan.chatDriverClassName);
    }

    private static void retransformClientClass(final String className) {
        final Instrumentation currentInstrumentation = instrumentation;

        if (currentInstrumentation == null)
            return;

        Thread thread = new Thread(new Runnable() {
            @Override
            public void run() {
                for (Class<?> type : currentInstrumentation.getAllLoadedClasses()) {
                    if (!className.equals(type.getName().replace('.', '/')) || !currentInstrumentation.isModifiableClass(type))
                        continue;

                    try {
                        currentInstrumentation.retransformClasses(type);
                    } catch (Throwable exception) {
                        recordIndexFailure(className, "Client-thread retransformation failed: " + exception.getMessage());
                    }

                    return;
                }
            }
        }, "Void client agent client-thread instrumentation");
        thread.setDaemon(true);
        thread.start();
    }

    static synchronized void recordIndexFailure(String location, String message) {
        IndexFailures.put(location, message);
        System.err.println("Void client agent UI index failed for " + location + ": " + message);
    }

    static String connectJson(String requestId, String address) {
        return await(new PendingOperation(requestId, "connect", address));
    }

    static String chatJson(String requestId, String message) {
        return await(new PendingOperation(requestId, "chat", message));
    }

    static synchronized void cancel(String requestId) {
        if (pending != null && pending.requestId.equals(requestId)) {
            complete(pending, error("request.cancelled", "The agent operation was cancelled"));
            pending = null;
        }
    }

    private static String await(PendingOperation operation) {
        synchronized (GameAutomationController.class) {
            if (pending != null)
                return error("request.pending", "Another game automation operation is already pending");

            if (LoaderIndexes.isEmpty())
                return error("code.discovery", "Minecraft UI bytecode has not been indexed; failures=" + IndexFailures);

            pending = operation;
            consumedScreen = null;
        }

        Object receiver = screenSetterReceiver.get();

        if (receiver instanceof Executor) {
            final Object client = receiver;
            System.err.println("Void client agent dispatching " + operation.kind + " through the Minecraft client executor");
            try {
                ((Executor) receiver).execute(new Runnable() {
                    @Override
                    public void run() {
                        System.err.println("Void client agent executing dispatched " + operation.kind + " on " + Thread.currentThread().getName());
                        Dispatched.set(Boolean.TRUE);

                        try {
                            applyClient(client);
                        } finally {
                            Dispatched.remove();
                        }
                    }
                });
            } catch (Throwable exception) {
                synchronized (GameAutomationController.class) {
                    pending = null;
                }

                return error("ui.dispatch", exception.getClass().getName() + ": " + exception.getMessage());
            }
        }

        try {
            operation.completed.await();
            return operation.response;
        } catch (InterruptedException exception) {
            Thread.currentThread().interrupt();
            cancel(operation.requestId);
            return error("request.interrupted", "The agent operation was interrupted");
        }
    }

    public static void applyScreen(Object screen) {
        if (screen == null || consumedScreen == screen || Boolean.TRUE.equals(Applying.get()))
            return;

        PendingOperation operation;
        GameAutomationIndex.IndexedCode index = index(screen.getClass().getClassLoader());

        synchronized (GameAutomationController.class) {
            clientThread = Thread.currentThread();
            if (index != null && screenSetterReceiver.get() == null)
                screenSetterReceiver = new WeakReference<Object>(findObject(screen, index.plan.clientClassName, 3));

            operation = pending;
        }

        if (operation == null || index == null || consumedScreen == screen)
            return;

        if (!isCurrentScreen(screenSetterReceiver.get(), screen, index.plan.screenBaseName))
            return;

        Applying.set(Boolean.TRUE);

        try {
            if ("connect".equals(operation.kind))
                applyConnect(screen, index, operation);
        } catch (Throwable exception) {
            fail(operation, "ui.action", exception, index, screen);
        } finally {
            Applying.remove();
        }
    }

    public static void applyClient(Object client) {
        if (Boolean.TRUE.equals(Applying.get()))
            return;

        PendingOperation operation;
        GameAutomationIndex.IndexedCode index = index(client.getClass().getClassLoader());

        if (index == null)
            return;

        synchronized (GameAutomationController.class) {
            if (isInstance(client, index.plan.clientClassName))
                screenSetterReceiver = new WeakReference<Object>(client);

            operation = pending;

            if (operation == null)
                return;

            if ("chat".equals(operation.kind) && !Boolean.TRUE.equals(Dispatched.get())
                && !isInstance(client, index.plan.chatDriverClassName))
                return;

            if (Boolean.TRUE.equals(Dispatched.get()))
                clientThread = Thread.currentThread();
            else if (clientThread == null)
                clientThread = Thread.currentThread();

            if (Thread.currentThread() != clientThread)
                return;

            if (screenSetterReceiver.get() == null)
                screenSetterReceiver = new WeakReference<Object>(findObject(client, index.plan.clientClassName, 3));
        }

        if ("connect".equals(operation.kind)) {
            Object screen = findCurrentScreen(screenSetterReceiver.get(), index.plan.screenBaseName);

            if (screen != null)
                applyScreen(screen);

            return;
        }

        if (!"chat".equals(operation.kind))
            return;

        Applying.set(Boolean.TRUE);

        try {
            Object chatScreen = constructChatScreen(index.plan.chat, client.getClass().getClassLoader());
            setScreen(index.plan, chatScreen);
            submitChat(chatScreen, index.plan.chat, operation.value);
            setScreen(index.plan, null);

            synchronized (GameAutomationController.class) {
                pending = null;
            }

            complete(operation, "{\"status\":\"ok\",\"stage\":\"chat.submit\",\"value\":" + quote(operation.value) + "}");
        } catch (Throwable exception) {
            fail(operation, "chat.submit", exception, index, null);
        } finally {
            Applying.remove();
        }
    }

    static synchronized void playerObserved(Object player) {
        if (pending == null || !"connect".equals(pending.kind) || !pending.submitted)
            return;

        PendingOperation operation = pending;
        pending = null;
        clientThread = null;
        complete(operation, "{\"status\":\"ok\",\"stage\":\"world.ready\",\"value\":" + quote(operation.value) + "}");
    }

    private static void applyConnect(Object screen, GameAutomationIndex.IndexedCode index, PendingOperation operation) throws Exception {
        String className = screen.getClass().getName().replace('.', '/');

        if (index.plan.directScreenName.equals(className)) {
            String exact = DirectConnectAccess.connect(screen, index.plan.directConnect, operation.value);

            if (!operation.value.equals(exact)) {
                synchronized (GameAutomationController.class) {
                    pending = null;
                }

                complete(operation, error("address.verify", "Minecraft returned " + quote(exact) + " instead of " + quote(operation.value)));
                return;
            }

            operation.submitted = true;
            consumedScreen = screen;
            return;
        }

        if (operation.submitted) {
            consumedScreen = screen;
            return;
        }

        TransitionPlan transition = selectTransition(index, className);

        if (transition == null)
        {
            consumedScreen = screen;
            if (!className.equals(waitingScreenName))
            {
                waitingScreenName = className;
                System.err.println("Void client agent is waiting for a routable screen after " + className);
            }
            return;
        }

        waitingScreenName = null;
        System.err.println("Void client agent invoking UI transition " + transition.describe());
        if (invokeTransition(screen, transition)) {
            consumedScreen = screen;
        }
    }

    private static TransitionPlan selectTransition(GameAutomationIndex.IndexedCode index, String source) {
        List<TransitionPlan> transitions = transitions(index, source);

        if (transitions.isEmpty())
            return null;

        List<TransitionPlan> candidates = new ArrayList<TransitionPlan>();
        int bestDistance = Integer.MAX_VALUE;

        for (TransitionPlan transition : transitions) {
            int distance = distanceToDirect(index, transition, new HashSet<String>());

            if (distance < bestDistance) {
                candidates.clear();
                candidates.add(transition);
                bestDistance = distance;
            } else if (distance == bestDistance) {
                candidates.add(transition);
            }
        }

        if (bestDistance == Integer.MAX_VALUE) {
            List<TransitionPlan> parentCandidates = new ArrayList<TransitionPlan>();

            for (TransitionPlan transition : transitions) {
                if (transition.targetsParent)
                    parentCandidates.add(transition);
            }

            if (parentCandidates.size() > 1) {
                List<TransitionPlan> enabling = new ArrayList<TransitionPlan>();

                for (TransitionPlan transition : parentCandidates) {
                    if (transition.enablesTransition)
                        enabling.add(transition);
                }

                parentCandidates = enabling;
            }

            return parentCandidates.size() == 1 ? parentCandidates.get(0) : null;
        }

        return candidates.size() == 1 ? candidates.get(0) : null;
    }

    private static int distanceToDirect(GameAutomationIndex.IndexedCode index, TransitionPlan transition, Set<String> visited) {
        int best = Integer.MAX_VALUE;

        for (String target : transition.targetScreenNames) {
            if (index.plan.directScreenName.equals(target))
                return 1;

            int nested = distanceFrom(index, target, visited);

            if (nested != Integer.MAX_VALUE)
                best = Math.min(best, nested + 1);
        }

        return best;
    }

    private static int distanceFrom(GameAutomationIndex.IndexedCode index, String source, Set<String> visited) {
        if (!visited.add(source))
            return Integer.MAX_VALUE;

        int best = Integer.MAX_VALUE;

        for (TransitionPlan transition : transitions(index, source))
            best = Math.min(best, distanceToDirect(index, transition, visited));

        visited.remove(source);
        return best;
    }

    private static List<TransitionPlan> transitions(GameAutomationIndex.IndexedCode index, String className) {
        String current = className;

        while (current != null) {
            List<TransitionPlan> transitions = index.plan.transitions.get(current);

            if (transitions != null)
                return transitions;

            current = index.superTypes.get(current);
        }

        return Collections.emptyList();
    }

    private static boolean invokeTransition(Object screen, TransitionPlan transition) throws Exception {
        ClassLoader loader = screen.getClass().getClassLoader();
        Class<?> owner = loadClass(loader, transition.owner);
        TypeArguments arguments = resolveArguments(loader, transition.methodDescriptor);
        Method method = owner.getDeclaredMethod(transition.methodName, arguments.types);
        method.setAccessible(true);
        Object control = transition.controlId == null ? null : findControl(screen, transition, 4,
            Collections.newSetFromMap(new IdentityHashMap<Object, Boolean>()));

        if (transition.controlId != null && control == null)
            return false;

        for (int index = 0; index < arguments.values.length; index++) {
            Class<?> parameter = arguments.types[index];

            if (parameter.isInstance(screen))
                arguments.values[index] = screen;
            else if (control != null && parameter.isInstance(control))
                arguments.values[index] = control;
            else if (parameter == Boolean.TYPE)
                arguments.values[index] = Boolean.TRUE;
            else if (parameter == Integer.TYPE)
                arguments.values[index] = Integer.valueOf(0);
        }

        method.invoke(transition.isStatic ? null : screen, arguments.values);
        return true;
    }

    private static Object findControl(Object value, TransitionPlan transition, int depth, Set<Object> visited) throws Exception {
        if (value == null || depth < 0 || !visited.add(value))
            return null;

        if (isInstance(value, transition.controlClassName)) {
            Field id = declaredField(loadClass(value.getClass().getClassLoader(), transition.controlClassName), transition.controlIdFieldName);

            if (transition.controlId.equals(id.get(value)))
                return value;
        }

        if (value instanceof Collection<?>) {
            for (Object item : (Collection<?>) value) {
                Object result = findControl(item, transition, depth - 1, visited);

                if (result != null)
                    return result;
            }

            return null;
        }

        if (value.getClass().isArray()) {
            for (int index = 0; index < Array.getLength(value); index++) {
                Object result = findControl(Array.get(value, index), transition, depth - 1, visited);

                if (result != null)
                    return result;
            }

            return null;
        }

        for (Class<?> type = value.getClass(); type != null && type != Object.class; type = type.getSuperclass()) {
            for (Field field : type.getDeclaredFields()) {
                if (Modifier.isStatic(field.getModifiers()) || field.getType().isPrimitive()
                    || field.getType().getName().startsWith("java.lang."))
                    continue;

                field.setAccessible(true);
                Object result = findControl(field.get(value), transition, depth - 1, visited);

                if (result != null)
                    return result;
            }
        }

        return null;
    }

    private static Object constructChatScreen(ChatPlan plan, ClassLoader loader) throws Exception {
        TypeArguments arguments = resolveArguments(loader, plan.constructorDescriptor);
        Constructor<?> constructor = loadClass(loader, plan.screenClassName).getDeclaredConstructor(arguments.types);
        constructor.setAccessible(true);

        for (int index = 0; index < arguments.values.length; index++)
            arguments.values[index] = plan.constructorArguments[index];

        return constructor.newInstance(arguments.values);
    }

    private static void submitChat(Object screen, ChatPlan plan, String message) throws Exception {
        ClassLoader loader = screen.getClass().getClassLoader();
        Field textField = declaredField(screen.getClass(), plan.textFieldName);
        Object widget = textField.get(screen);
        Method setter = loadClass(loader, plan.textSetterOwner).getDeclaredMethod(plan.textSetterName, String.class);
        setter.setAccessible(true);
        setter.invoke(widget, message);
        Method getter = loadClass(loader, plan.textGetterOwner).getDeclaredMethod(plan.textGetterName);
        getter.setAccessible(true);
        String exact = (String) getter.invoke(widget);

        if (!message.equals(exact))
            throw new IllegalStateException("Minecraft returned " + quote(exact) + " instead of " + quote(message));

        TypeArguments arguments = resolveArguments(loader, plan.submitDescriptor);

        for (int index = 0; index < arguments.types.length; index++) {
            if (arguments.types[index] == String.class)
                arguments.values[index] = exact;
            else if (arguments.types[index] == Boolean.TYPE)
                arguments.values[index] = Boolean.TRUE;
        }

        Method submit = loadClass(loader, plan.submitOwner).getDeclaredMethod(plan.submitName, arguments.types);
        submit.setAccessible(true);
        submit.invoke(screen, arguments.values);
    }

    private static void setScreen(AutomationPlan plan, Object screen) throws Exception {
        Object receiver = screenSetterReceiver.get();

        if (receiver == null)
            throw new IllegalStateException("The Minecraft screen setter receiver is unavailable");

        ClassLoader loader = receiver.getClass().getClassLoader();
        Class<?> screenBase = loadClass(loader, plan.screenBaseName);
        Method setter = loadClass(loader, plan.clientClassName).getDeclaredMethod(plan.screenSetterName, screenBase);
        setter.setAccessible(true);
        setter.invoke(receiver, screen);
    }

    private static Object findObject(Object root, String internalName, int depth) {
        if (root == null || depth < 0)
            return null;

        if (isInstance(root, internalName))
            return root;

        Set<Object> visited = Collections.newSetFromMap(new IdentityHashMap<Object, Boolean>());
        List<Object> current = new ArrayList<Object>();
        current.add(root);

        for (int level = 0; level <= depth; level++) {
            List<Object> next = new ArrayList<Object>();

            for (Object value : current) {
                if (value == null || !visited.add(value))
                    continue;

                if (isInstance(value, internalName))
                    return value;

                for (Class<?> type = value.getClass(); type != null && type != Object.class; type = type.getSuperclass()) {
                    for (Field field : type.getDeclaredFields()) {
                        if (Modifier.isStatic(field.getModifiers()) || field.getType().isPrimitive())
                            continue;

                        try {
                            field.setAccessible(true);
                            next.add(field.get(value));
                        } catch (Throwable exception) {
                            // Inaccessible unrelated state is not part of the screen path.
                        }
                    }
                }
            }

            current = next;
        }

        return null;
    }

    private static boolean isCurrentScreen(Object client, Object screen, String screenBaseName) {
        return findCurrentScreen(client, screenBaseName) == screen;
    }

    private static Object findCurrentScreen(Object client, String screenBaseName) {
        if (client == null)
            return null;

        try {
            Class<?> screenBase = loadClass(client.getClass().getClassLoader(), screenBaseName);

            for (Class<?> type = client.getClass(); type != null && type != Object.class; type = type.getSuperclass()) {
                for (Field field : type.getDeclaredFields()) {
                    if (Modifier.isStatic(field.getModifiers()) || !screenBase.isAssignableFrom(field.getType()))
                        continue;

                    field.setAccessible(true);

                    Object value = field.get(client);

                    if (value != null)
                        return value;
                }
            }
        } catch (Throwable exception) {
            return null;
        }

        return null;
    }

    private static GameAutomationIndex.IndexedCode index(ClassLoader loader) {
        return LoaderIndexes.get(loader);
    }

    private static boolean isInstance(Object value, String internalName) {
        if (value == null || internalName == null)
            return false;

        try {
            return loadClass(value.getClass().getClassLoader(), internalName).isInstance(value);
        } catch (Throwable exception) {
            return false;
        }
    }

    private static Class<?> loadClass(ClassLoader loader, String internalName) throws ClassNotFoundException {
        return Class.forName(internalName.replace('/', '.'), false, loader);
    }

    private static Field declaredField(Class<?> owner, String name) throws NoSuchFieldException {
        for (Class<?> type = owner; type != null; type = type.getSuperclass()) {
            try {
                Field field = type.getDeclaredField(name);
                field.setAccessible(true);
                return field;
            } catch (NoSuchFieldException exception) {
                // Continue through inherited screen fields.
            }
        }

        throw new NoSuchFieldException(owner.getName() + '.' + name);
    }

    private static TypeArguments resolveArguments(ClassLoader loader, String descriptor) throws ClassNotFoundException {
        Type[] argumentTypes = org.objectweb.asm.Type.getArgumentTypes(descriptor);
        Class<?>[] types = new Class<?>[argumentTypes.length];
        Object[] values = new Object[argumentTypes.length];

        for (int index = 0; index < argumentTypes.length; index++) {
            Type type = argumentTypes[index];

            switch (type.getSort()) {
                case Type.BOOLEAN:
                    types[index] = Boolean.TYPE;
                    values[index] = Boolean.FALSE;
                    break;
                case Type.BYTE:
                    types[index] = Byte.TYPE;
                    values[index] = Byte.valueOf((byte) 0);
                    break;
                case Type.CHAR:
                    types[index] = Character.TYPE;
                    values[index] = Character.valueOf((char) 0);
                    break;
                case Type.SHORT:
                    types[index] = Short.TYPE;
                    values[index] = Short.valueOf((short) 0);
                    break;
                case Type.INT:
                    types[index] = Integer.TYPE;
                    values[index] = Integer.valueOf(0);
                    break;
                case Type.LONG:
                    types[index] = Long.TYPE;
                    values[index] = Long.valueOf(0);
                    break;
                case Type.FLOAT:
                    types[index] = Float.TYPE;
                    values[index] = Float.valueOf(0);
                    break;
                case Type.DOUBLE:
                    types[index] = Double.TYPE;
                    values[index] = Double.valueOf(0);
                    break;
                case Type.ARRAY:
                    types[index] = Class.forName(type.getDescriptor().replace('/', '.'), false, loader);
                    break;
                default:
                    types[index] = loadClass(loader, type.getInternalName());
                    break;
            }
        }

        return new TypeArguments(types, values);
    }

    private static void fail(PendingOperation operation, String stage, Throwable exception,
                             GameAutomationIndex.IndexedCode index, Object screen) {
        while (exception instanceof java.lang.reflect.InvocationTargetException
            && ((java.lang.reflect.InvocationTargetException) exception).getTargetException() != null)
            exception = ((java.lang.reflect.InvocationTargetException) exception).getTargetException();

        synchronized (GameAutomationController.class) {
            if (pending == operation)
                pending = null;
        }

        String screenName = screen == null ? "none" : screen.getClass().getName();
        complete(operation, error(stage, exception.getClass().getName() + ": " + exception.getMessage()
            + "; screen=" + screenName + "; direct=" + index.plan.directScreenName));
    }

    private static void complete(PendingOperation operation, String response) {
        operation.response = response;
        operation.completed.countDown();
    }

    private static String error(String stage, String message) {
        return "{\"status\":\"unavailable\",\"stage\":" + quote(stage) + ",\"message\":" + quote(message) + "}";
    }

    private static String quote(String value) {
        if (value == null)
            return "null";

        StringBuilder result = new StringBuilder(value.length() + 2).append('"');

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

    private static final class PendingOperation {
        final String requestId;
        final String kind;
        final String value;
        final CountDownLatch completed = new CountDownLatch(1);
        volatile String response;
        volatile boolean submitted;

        PendingOperation(String requestId, String kind, String value) {
            this.requestId = requestId;
            this.kind = kind;
            this.value = value;
        }
    }

    private static final class TypeArguments {
        final Class<?>[] types;
        final Object[] values;

        TypeArguments(Class<?>[] types, Object[] values) {
            this.types = types;
            this.values = values;
        }
    }
}
