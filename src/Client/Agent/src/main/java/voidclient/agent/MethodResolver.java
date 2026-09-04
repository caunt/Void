package voidclient.agent;

import java.lang.reflect.Method;
import java.util.HashSet;
import java.util.Set;

final class MethodResolver {
    private MethodResolver() {
    }

    static Method resolve(Class<?> owner, String name, Class<?>... parameterTypes) throws NoSuchMethodException {
        for (Class<?> type = owner; type != null; type = type.getSuperclass()) {
            Method method = findDeclared(type, name, parameterTypes);

            if (method != null)
                return method;
        }

        Set<Class<?>> visited = new HashSet<Class<?>>();

        for (Class<?> type = owner; type != null; type = type.getSuperclass()) {
            Method method = findInterfaces(type.getInterfaces(), name, parameterTypes, visited);

            if (method != null)
                return method;
        }

        throw new NoSuchMethodException(describe(owner, name, parameterTypes));
    }

    private static Method findDeclared(Class<?> owner, String name, Class<?>[] parameterTypes) {
        try {
            Method method = owner.getDeclaredMethod(name, parameterTypes);
            method.setAccessible(true);
            return method;
        } catch (NoSuchMethodException exception) {
            return null;
        }
    }

    private static Method findInterfaces(Class<?>[] interfaces, String name, Class<?>[] parameterTypes, Set<Class<?>> visited) {
        for (Class<?> type : interfaces) {
            if (!visited.add(type))
                continue;

            Method method = findDeclared(type, name, parameterTypes);

            if (method != null)
                return method;
        }

        for (Class<?> type : interfaces) {
            Method method = findInterfaces(type.getInterfaces(), name, parameterTypes, visited);

            if (method != null)
                return method;
        }

        return null;
    }

    private static String describe(Class<?> owner, String name, Class<?>[] parameterTypes) {
        StringBuilder description = new StringBuilder(owner.getName()).append('.').append(name).append('(');

        for (int index = 0; index < parameterTypes.length; index++) {
            if (index > 0)
                description.append(',');

            description.append(parameterTypes[index].getTypeName());
        }

        return description.append(')').toString();
    }
}
