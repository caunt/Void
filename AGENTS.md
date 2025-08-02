# Repository Guidelines

## Structure

This repository is a .NET solution composed of the following sections:

- src — all production code, including:
  - Api — API abstractions.
  - Benchmarks — benchmark projects.
  - Debug — debug harness and utilities.
  - Minecraft — Minecraft-specific libraries.
  - Platform — main proxy entry point.
  - Playground — sample playground app.
  - Plugins — built-in plugins organized by feature:
    - Common — shared infrastructure and helpers for other plugins.
    - Essentials — debugging and moderation tools.
    - ForwardingSupport — server forwarding protocol support (e.g., Velocity).
    - ModsSupport — modded client integrations such as Forge.
    - ProtocolSupport — compatibility with multiple protocol versions.
    - Watchdog — HTTP service that monitors proxy health.
    - ExamplePlugin — minimal API sample plugin.
  - Servers — server implementation integrations such as Bukkit.
  - Terminal — terminal UI for proxy console.
- tests — xUnit test projects.
- docs — documentation site built with Astro.
- pdk — plugin development kit example.
- Void.slnx — solution file referencing all projects.

## Coding conventions

Follow the existing C# style rules:

- Private fields prefixed with an underscore and method parameters use camelCase.
- Braces sit on their own lines with a blank line before loops and conditional statements.
- Method signatures remain on a single line.
- Identifier names are fully descriptive and never abbreviated.
- Async methods carry an `Async` suffix.
- Pattern matching is preferred over explicit casts.
- `nameof` is used for parameter checks, logging, and exceptions.
- Never use the null-forgiving `!` operator to silence possible null reference warnings.

## Development

- Run `dotnet format` before committing any code changes, then execute `dotnet test` to ensure all tests pass.
- Be patient with long-running tests and avoid aborting them early; some may take several minutes to complete.
- After changing source code, run `dotnet build` from the repository root.
- Conventional Commits are required for commit messages.
- Use only the following Conventional Commit types:
  - `feat` — Features
  - `fix` — Bug Fixes
  - `perf` — Performance Improvements
  - `deps` — Dependencies
  - `revert` — Reverts
  - `docs` — Documentation
  - `style` — Styles
  - `chore` — Miscellaneous Chores
  - `refactor` — Code Refactoring
  - `test` — Tests
  - `build` — Build System
  - `ci` — Continuous Integration
- Include in the commit description a brief note about any observable behavior change.
- Use the `fix` or `feat` type only when your changes modify the proxy code in `./src`. For documentation, CI, or other unrelated updates, choose a more appropriate type such as `docs` or `chore`.
- Append a [gitmoji](https://gitmoji.dev/specification) after the commit scope, e.g., `feat(api): ✨ add new endpoint`.
- Pull request titles should follow the same Conventional Commits format.
- Never modify `CHANGELOG.md`; it is generated automatically by `release-please` from commit history.

## Protocol guidance

When adding or modifying packet handling:

- Identify the Minecraft version whose packets require updates.
- Look up the release date of the next minor update (for example, if targeting 1.21, note that 1.21.1 released on August 8, 2024).
- Open the [Java Edition protocol/Packets](https://minecraft.wiki/w/Java_Edition_protocol/Packets?action=history&limit=500) page and view its revision history.
- Select the last revision made before that next update's release date (for 1.21, use the August 5, 2024 revision).
- Use that snapshot as your reference when altering the target version's protocol.
- For sub-versions, repeat this process using the next sub-version's release date and its preceding revision.
- Check the `src/Plugins/ProtocolSupport` directory to see how existing packets are implemented and update them as necessary.
