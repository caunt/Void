package voidclient.agent;

import java.util.ArrayList;
import java.util.List;

final class AutomationPlan {
    final String screenBaseName;
    final String clientClassName;
    final String screenSetterName;
    final String directScreenName;
    final String chatDriverClassName;
    final DirectConnectPlan directConnect;
    final ChatPlan chat;
    final java.util.Map<String, List<TransitionPlan>> transitions;

    AutomationPlan(String screenBaseName, String clientClassName, String screenSetterName,
                   String directScreenName, String chatDriverClassName, DirectConnectPlan directConnect, ChatPlan chat,
                   java.util.Map<String, List<TransitionPlan>> transitions) {
        this.screenBaseName = screenBaseName;
        this.clientClassName = clientClassName;
        this.screenSetterName = screenSetterName;
        this.directScreenName = directScreenName;
        this.chatDriverClassName = chatDriverClassName;
        this.directConnect = directConnect;
        this.chat = chat;
        this.transitions = transitions;
    }

}

final class TransitionPlan {
    final String owner;
    final String methodName;
    final String methodDescriptor;
    final boolean isStatic;
    final List<String> targetScreenNames;
    final boolean targetsParent;
    final String controlClassName;
    final String controlIdFieldName;
    final Integer controlId;
    final boolean enablesTransition;

    TransitionPlan(String owner, String methodName, String methodDescriptor, boolean isStatic,
                   List<String> targetScreenNames, boolean targetsParent,
                   String controlClassName, String controlIdFieldName, Integer controlId,
                   boolean enablesTransition) {
        this.owner = owner;
        this.methodName = methodName;
        this.methodDescriptor = methodDescriptor;
        this.isStatic = isStatic;
        this.targetScreenNames = new ArrayList<String>(targetScreenNames);
        this.targetsParent = targetsParent;
        this.controlClassName = controlClassName;
        this.controlIdFieldName = controlIdFieldName;
        this.controlId = controlId;
        this.enablesTransition = enablesTransition;
    }

    String describe() {
        return owner + '.' + methodName + methodDescriptor + " targets=" + targetScreenNames
            + " parent=" + targetsParent + " controlId=" + controlId;
    }
}

final class ChatPlan {
    final String screenClassName;
    final String constructorDescriptor;
    final Object[] constructorArguments;
    final String textFieldName;
    final String textSetterOwner;
    final String textSetterName;
    final String textGetterOwner;
    final String textGetterName;
    final String submitOwner;
    final String submitName;
    final String submitDescriptor;

    ChatPlan(String screenClassName, String constructorDescriptor, Object[] constructorArguments,
             String textFieldName, String textSetterOwner, String textSetterName,
             String textGetterOwner, String textGetterName, String submitOwner,
             String submitName, String submitDescriptor) {
        this.screenClassName = screenClassName;
        this.constructorDescriptor = constructorDescriptor;
        this.constructorArguments = constructorArguments;
        this.textFieldName = textFieldName;
        this.textSetterOwner = textSetterOwner;
        this.textSetterName = textSetterName;
        this.textGetterOwner = textGetterOwner;
        this.textGetterName = textGetterName;
        this.submitOwner = submitOwner;
        this.submitName = submitName;
        this.submitDescriptor = submitDescriptor;
    }

    String describe() {
        return screenClassName + " text=" + textFieldName + " submit=" + submitOwner + '.' + submitName + submitDescriptor;
    }
}
