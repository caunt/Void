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
  - Private fields prefixed with an underscore and method parameters use camelCase
  - Braces sit on their own lines, with a blank line before loops and conditional statements
  - Method signatures remain on a single line
  - Identifier names are always fully descriptive and never abbreviated
  - Async methods carry an Async suffix
  - Pattern matching is favored over explicit casts
  - nameof is used for parameter checks, logging, and exceptions

## Development

- Run `dotnet format` before committing any code changes.
- After changing source code, run `dotnet build` from the repository root.
- Conventional Commits are required for commit messages.
- Use the `fix` or `feat` type only when your changes modify the proxy code in
  `./src`. For documentation, CI, or other unrelated updates, choose a more
  appropriate type such as `docs` or `chore`.
- Append a [gitmoji](https://gitmoji.dev/specification) after the commit scope,
  e.g., `feat(api): ✨ add new endpoint`.
- Pull request titles should follow the same Conventional Commits format.
- Never modify `CHANGELOG.md`; it is generated automatically by `release-please` from commit history.

## Protocol guidance

When adding or modifying packet handling:

- Reference <https://minecraft.wiki/w/Java_Edition_protocol/Packets> for the current protocol packet list and structures.
- For previous or historical packet behavior, view the edit history of that page.
- Check the `src/Plugins/ProtocolSupport` directory to see how existing packets are implemented and update them as necessary.
