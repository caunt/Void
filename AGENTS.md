# Repository Guidelines

## Structure

This repository is a .NET solution composed of the following sections:

- **src** — all production code, including:
  - **Api** — API abstractions.
  - **Benchmarks** — benchmark projects.
  - **Debug** — debug harness and utilities.
  - **Minecraft** — Minecraft-specific libraries.
  - **Platform** — main proxy entry point:
    - **Commands** — parses and executes player commands.
    - **Configurations** — loads settings and serializes configuration files.
    - **Console** — interactive console service.
    - **Crypto** — cryptographic utilities.
    - **Events** — internal event system.
    - **Extensions** — host builder helpers and other extensions.
    - **Links** — manages connections to upstream servers.
    - **Players** — tracks online players and sessions.
    - **Plugins** — plugin discovery and dependency resolution.
    - **Properties** — application resources and launch settings.
    - **Resources** — embedded default configuration files.
    - **Servers** — backend server list management.
    - **Utils** — miscellaneous helper utilities.
  - **Playground** — sample playground app.
  - **Plugins** — built-in plugins organized by feature:
    - **Common** — shared infrastructure and helpers for other plugins.
    - **Essentials** — debugging and moderation tools.
    - **ForwardingSupport** — server forwarding protocol support (e.g., Velocity).
    - **ModsSupport** — modded client integrations such as Forge.
    - **ProtocolSupport** — compatibility with multiple protocol versions.
    - **Watchdog** — HTTP service that monitors proxy health.
    - **ExamplePlugin** — minimal API sample plugin.
  - **Servers** — server implementation integrations such as Bukkit.
  - **Terminal** — terminal UI for proxy console.
- **tests** — xUnit test projects.
- **docs** — documentation site built with Astro.
- **pdk** — plugin development kit example.
- **Void.slnx** — solution file referencing all projects.

## Coding conventions

Follow the existing C# style rules:

- Private fields prefixed with an underscore and method parameters use camelCase.
- Braces sit on their own lines with a blank line before loops and conditional statements.
- Method signatures remain on a single line.
- Identifier names are fully descriptive and never abbreviated.
- Async methods carry an `Async` suffix.
- Pattern matching is preferred over explicit casts.
- `nameof` is used for parameter checks, logging, and exceptions.

## Development

- Run `dotnet format` before committing any code changes, then execute `dotnet test` to ensure all tests pass.
- After changing source code, run `dotnet build` from the repository root.
- Conventional Commits are required for commit messages.
- Use the `fix` or `feat` type only when your changes modify the proxy code in `./src`. For documentation, CI, or other unrelated updates, choose a more appropriate type such as `docs` or `chore`.
- Append a [gitmoji](https://gitmoji.dev/specification) after the commit scope, e.g., `feat(api): ✨ add new endpoint`.
- Pull request titles should follow the same Conventional Commits format.
- Never modify `CHANGELOG.md`; it is generated automatically by `release-please` from commit history.

## Protocol guidance

When adding or modifying packet handling:

- Reference <https://minecraft.wiki/w/Java_Edition_protocol/Packets> for the current protocol packet list and structures.
- For previous or historical packet behavior, view the edit history of that page.
- Check the `src/Plugins/ProtocolSupport` directory to see how existing packets are implemented and update them as necessary.
