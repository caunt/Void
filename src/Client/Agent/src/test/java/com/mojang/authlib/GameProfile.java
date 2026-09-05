package com.mojang.authlib;

public final class GameProfile {
    private final String name;

    public GameProfile() {
        this(null);
    }

    public GameProfile(String name) {
        this.name = name;
    }

    public String getName() {
        return name;
    }
}
