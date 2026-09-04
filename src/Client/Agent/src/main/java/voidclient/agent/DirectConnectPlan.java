package voidclient.agent;

final class DirectConnectPlan {
    final String screenClassName;
    final String textFieldName;
    final String textFieldDescriptor;
    final String setterOwner;
    final String setterName;
    final String getterOwner;
    final String getterName;
    final String serverDataFieldName;
    final String serverDataFieldDescriptor;
    final String addressOwner;
    final String addressFieldName;
    final String callbackFieldName;
    final String callbackFieldDescriptor;
    final String callbackOwner;
    final String callbackName;
    final String callbackDescriptor;

    DirectConnectPlan(String screenClassName, String textFieldName, String textFieldDescriptor,
                      String setterOwner, String setterName, String getterOwner, String getterName,
                      String serverDataFieldName, String serverDataFieldDescriptor, String addressOwner,
                      String addressFieldName, String callbackFieldName, String callbackFieldDescriptor,
                      String callbackOwner, String callbackName, String callbackDescriptor) {
        this.screenClassName = screenClassName;
        this.textFieldName = textFieldName;
        this.textFieldDescriptor = textFieldDescriptor;
        this.setterOwner = setterOwner;
        this.setterName = setterName;
        this.getterOwner = getterOwner;
        this.getterName = getterName;
        this.serverDataFieldName = serverDataFieldName;
        this.serverDataFieldDescriptor = serverDataFieldDescriptor;
        this.addressOwner = addressOwner;
        this.addressFieldName = addressFieldName;
        this.callbackFieldName = callbackFieldName;
        this.callbackFieldDescriptor = callbackFieldDescriptor;
        this.callbackOwner = callbackOwner;
        this.callbackName = callbackName;
        this.callbackDescriptor = callbackDescriptor;
    }

    String describe() {
        return screenClassName + " text=" + textFieldName + ':' + textFieldDescriptor
            + " address=" + serverDataFieldName + ':' + serverDataFieldDescriptor + "->" + addressOwner + '.' + addressFieldName
            + " callback=" + callbackFieldName + ':' + callbackFieldDescriptor + "->" + callbackOwner + '.' + callbackName + callbackDescriptor;
    }
}
