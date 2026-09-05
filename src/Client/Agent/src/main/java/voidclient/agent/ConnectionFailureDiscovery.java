package voidclient.agent;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import org.objectweb.asm.Opcodes;
import org.objectweb.asm.Type;
import org.objectweb.asm.tree.AbstractInsnNode;
import org.objectweb.asm.tree.ClassNode;
import org.objectweb.asm.tree.FieldNode;
import org.objectweb.asm.tree.MethodInsnNode;
import org.objectweb.asm.tree.MethodNode;

final class ConnectionFailureDiscovery {
    private ConnectionFailureDiscovery() {
    }

    static Map<String, List<String>> discover(Map<String, ClassNode> types) {
        Set<String> callbacks = new HashSet<String>();
        for (ClassNode type : types.values()) {
            if (!isNetworkHandler(types, type.name))
                continue;
            for (FieldNode listenerField : type.fields) {
                if (!listenerField.desc.startsWith("L"))
                    continue;
                ClassNode listener = types.get(listenerField.desc.substring(1, listenerField.desc.length() - 1));
                if (listener == null || (listener.access & Opcodes.ACC_INTERFACE) == 0)
                    continue;
                for (MethodNode callback : listener.methods) {
                    Type[] arguments = Type.getArgumentTypes(callback.desc);
                    if (arguments.length != 1 || arguments[0].getSort() != Type.OBJECT
                        || Type.getReturnType(callback.desc).getSort() != Type.VOID)
                        continue;
                    for (FieldNode reason : type.fields)
                        if (reason.desc.equals(arguments[0].getDescriptor()))
                            callbacks.add(listener.name + '.' + callback.name + callback.desc);
                }
            }
            for (MethodNode method : type.methods)
                for (AbstractInsnNode instruction : method.instructions) {
                    if (!(instruction instanceof MethodInsnNode))
                        continue;
                    MethodInsnNode call = (MethodInsnNode) instruction;
                    Type[] arguments = Type.getArgumentTypes(call.desc);
                    if (call.getOpcode() != Opcodes.INVOKEINTERFACE || arguments.length != 1
                        || arguments[0].getSort() != Type.OBJECT || Type.getReturnType(call.desc).getSort() != Type.VOID)
                        continue;
                    ClassNode owner = types.get(call.owner);
                    if (owner == null || (owner.access & Opcodes.ACC_INTERFACE) == 0)
                        continue;
                    for (FieldNode field : type.fields)
                        if (field.desc.equals("L" + call.owner + ";"))
                            callbacks.add(call.owner + '.' + call.name + call.desc);
                }
        }
        Map<String, List<String>> result = new HashMap<String, List<String>>();
        for (ClassNode type : types.values())
            for (MethodNode method : type.methods) {
                if ((method.access & (Opcodes.ACC_STATIC | Opcodes.ACC_ABSTRACT | Opcodes.ACC_NATIVE)) != 0)
                    continue;
                for (String callback : callbacks) {
                    int separator = callback.indexOf('.');
                    if (!callback.substring(separator + 1).equals(method.name + method.desc)
                        || !inherits(types, type.name, callback.substring(0, separator), new HashSet<String>()))
                        continue;
                    List<String> methods = result.get(type.name);
                    if (methods == null) {
                        methods = new ArrayList<String>();
                        result.put(type.name, methods);
                    }
                    methods.add(method.name + method.desc);
                }
            }
        if (result.isEmpty())
            throw new IllegalStateException("No native network-listener rejection callback was discovered");
        return result;
    }

    static boolean isNetworkHandler(Map<String, ClassNode> types, String name) {
        return inherits(types, name, "io/netty/channel/SimpleChannelInboundHandler", new HashSet<String>())
            || inherits(types, name, "io/netty/channel/ChannelInboundHandlerAdapter", new HashSet<String>());
    }

    private static boolean inherits(Map<String, ClassNode> types, String name, String target, Set<String> visited) {
        if (target.equals(name) || name != null && target.startsWith("io/netty/") && name.endsWith("/" + target))
            return true;
        if (name == null || !visited.add(name))
            return false;
        ClassNode type = types.get(name);
        if (type == null)
            return false;
        if (inherits(types, type.superName, target, visited))
            return true;
        for (String parent : type.interfaces)
            if (inherits(types, parent, target, visited))
                return true;
        return false;
    }
}
