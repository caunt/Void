# Copilot Instructions

Copilot must obey the repository guidelines defined in the root [**AGENTS.md**](../AGENTS.md). Ensure all suggestions, commits, and tests comply with those rules.

## Project Overview

Void is a high-performance Minecraft proxy designed for modded servers. It provides a flexible platform with support for multiple protocol versions, modded client integrations (e.g., Forge), server forwarding protocols (e.g., Velocity), and plugin development capabilities.

**Key Features:**
- Multi-protocol version support
- Modded client compatibility (Forge, etc.)
- Server forwarding protocol support (Velocity)
- Plugin system with API abstractions
- Terminal UI for proxy console
- HTTP monitoring service (Watchdog)

## Tech Stack

- **Language:** C# (.NET)
- **Solution File:** Void.slnx (references all projects)
- **Testing:** xUnit for unit tests
- **Documentation:** Astro-based documentation site
- **Build System:** .NET CLI (`dotnet`)

## Project Structure

This repository is organized as follows:

- **src/** — all production code:
  - **Api/** — API abstractions for plugins
  - **Benchmarks/** — performance benchmark projects
  - **Debug/** — debug harness and utilities
  - **Minecraft/** — Minecraft-specific libraries
  - **Platform/** — main proxy entry point
  - **Playground/** — sample playground application
  - **Plugins/** — built-in plugins organized by feature:
    - Common — shared infrastructure for plugins
    - Essentials — debugging and moderation tools
    - ForwardingSupport — server forwarding protocols
    - ModsSupport — modded client integrations
    - ProtocolSupport — multi-version protocol compatibility
    - Watchdog — HTTP health monitoring service
    - ExamplePlugin — minimal API sample plugin
  - **Servers/** — server implementation integrations (e.g., Bukkit)
  - **Terminal/** — terminal UI components
  - **Tests/** — xUnit test projects
- **docs/** — documentation site built with Astro
- **pdk/** — plugin development kit example
- **Void.slnx** — solution file

## Development Workflow

### Building the Project
```bash
dotnet build
```
Run this command from the repository root after making any source code changes.

### Running Tests
```bash
dotnet test
```
**Important:**
- Run tests whenever C# source files (`*.cs`) are modified
- Skip testing if only documentation or non-code files change
- Be patient with long-running tests; some may take several minutes
- Never abort tests early

### Coding Standards

Follow the C# style rules defined in [**AGENTS.md**](../AGENTS.md):

- **Indentation:** Four spaces (tab width = 4)
- **Naming:**
  - Private fields: `_camelCase` (underscore prefix)
  - Method parameters: `camelCase`
  - Identifiers: Fully descriptive, never abbreviated
  - Async methods: `Async` suffix
- **Braces:** On their own lines with blank line before loops/conditionals
- **Method signatures:** Single line
- **Pattern matching:** Preferred over explicit casts
- **`nameof`:** Used for parameter checks, logging, exceptions
- **Null safety:** Never use `!` operator to silence null warnings

### Commit Guidelines

**Format:** `type(scope): gitmoji description`

**Required Elements:**
1. **Type:** One of: `feat`, `fix`, `perf`, `deps`, `revert`, `docs`, `style`, `chore`, `refactor`, `test`, `build`, `ci`
2. **Scope:** Always required after type
3. **Gitmoji:** After scope (see [**gitmoji.dev**](https://gitmoji.dev/))
4. **Body:** Required, describing observable behavior changes

**Examples:**
- `feat(api): ✨ add new endpoint`
- `fix(protocol): 🐛 correct packet handling`
- `docs(readme): 📝 update installation guide`
- `ci(workflows): 🔧 fix build`

**Rules:**
- Use `fix` or `feat` only for proxy code in `./src`
- Use appropriate type for docs, CI, or other changes
- Commit only actual code changes (preserve CRLF line endings)
- Never modify `CHANGELOG.md` (auto-generated)

### Pull Request Requirements

**Title:** Follow Conventional Commits format with scope and gitmoji (same as commit messages)

**Description Template:**
```markdown
## Summary
One-sentence problem and outcome.

## Rationale
Why this is needed; alternatives considered briefly.

## Changes
Concise, high-signal description of what changed.

## Verification
How it was tested; include commands or steps.

## Performance
Before/after numbers if relevant; memory/alloc notes.

## Risks & Rollback
Known risks, how to revert safely.

## Breaking/Migration
Required actions for users, if any.

## Links
Issues, discussions, specs.
```

## Documentation Standards

- **Link formatting:** Make link text bold: `[**link text**](url)`
- **Code in links:** Never use backticks inside link captions; keep code formatting outside
- **Protocol updates:** Reference specific revisions from [**Java Edition protocol/Packets**](https://minecraft.wiki/w/Java_Edition_protocol/Packets)

## Testing & Validation

- Run `dotnet build` after code changes
- Run `dotnet test` when C# files are modified
- Check tests in `src/Tests/` directory for examples
- Verify all existing tests pass before submitting changes

## Key Resources

- [**AGENTS.md**](../AGENTS.md) — Complete repository guidelines
- [**README.md**](../README.md) — Project overview and quick start
- [**Documentation Site**](https://void.caunt.world/) — User guides and API docs
- [**Example Plugin**](../src/Plugins/ExamplePlugin/ExamplePlugin.cs) — Plugin development reference

## What Not to Modify

- **CHANGELOG.md** — Auto-generated by release-please
- **.github/agents/** — Contains instructions for other agents (not accessible to you)
