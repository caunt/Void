package voidclient.agent;

final class RotationSources {
    final PositionSource bodyYaw;
    final PositionSource headYaw;
    final PositionSource headPitch;

    RotationSources(PositionSource bodyYaw, PositionSource headYaw, PositionSource headPitch) {
        this.bodyYaw = bodyYaw;
        this.headYaw = headYaw;
        this.headPitch = headPitch;
    }

    boolean isComplete() {
        return bodyYaw != null && headYaw != null && headPitch != null;
    }
}
