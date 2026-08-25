package voidclient.agent;

final class PositionSource {
    final String owner;
    final String name;
    final boolean method;

    private PositionSource(String owner, String name, boolean method) {
        this.owner = owner;
        this.name = name;
        this.method = method;
    }

    static PositionSource field(String owner, String name) {
        return new PositionSource(owner, name, false);
    }

    static PositionSource method(String owner, String name) {
        return new PositionSource(owner, name, true);
    }
}
