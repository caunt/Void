# Repository Guidelines

This repository is a .NET solution structured as follows:

- `src/` – All production source code.
  - `Api/` – API abstractions.
  - `Benchmarks/` – Benchmark projects.
  - `Debug/` – Debug harness and utilities.
  - `Minecraft/` – Minecraft specific libraries.
  - `Platform/` – Main proxy entry point:
    - `Commands/` – Handles parsing and executing player commands.
    - `Configurations/` – Loads settings and serializes configuration files.
    - `Console/` – Implements the interactive console service.
    - `Crypto/` – Provides cryptographic utilities.
    - `Events/` – Publishes and listens for internal events.
    - `Extensions/` – Host builder helpers and other extensions.
    - `Links/` – Manages connections to upstream servers.
    - `Players/` – Tracks online players and sessions.
    - `Plugins/` – Discovers plugins and resolves dependencies.
    - `Properties/` – Application resources and launch settings.
    - `Resources/` – Embedded default configuration files.
    - `Servers/` – Maintains the list of backend servers.
    - `Utils/` – Miscellaneous helper utilities.
  - `Playground/` – Sample playground app.
  - `Plugins/` – Built‑in plugins organized by feature:
    - `Common/` – Shared infrastructure and helpers for other plugins.
    - `Essentials/` – Core utilities such as debugging and moderation.
    - `ForwardingSupport/` – Support for server forwarding protocols (e.g., Velocity).
    - `ModsSupport/` – Integrations for modded clients like Forge.
    - `ProtocolSupport/` – Compatibility with multiple protocol versions.
    - `Watchdog/` – HTTP service that monitors proxy health.
    - `ExamplePlugin/` – Minimal sample plugin demonstrating APIs.
  - `Servers/` – Integrations with server implementations such as Bukkit.
  - `Terminal/` – Terminal UI for user interactions with the proxy console.
- `tests/` – xUnit test projects.
- `docs/` – Documentation site built with Astro.
- `pdk/` – Plugin development kit example project.
- `Void.slnx` – Solution file referencing all projects.

## Coding conventions

Follow the existing C# style rules:
  - Use descriptive names (no single-letter variables)
  - Insert a blank line before each if or foreach block
  - Keep method signatures on single line

## Development

- Run `dotnet format` before committing any code changes.
- After changing source code, run `dotnet build` from the repository root.
- Conventional Commits are required for commit messages.
