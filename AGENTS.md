# Repository Guidelines

## Structure

This repository is organized around these root components:

- `.github` — GitHub Actions workflows, issue templates, and repository automation.
- `demo` — sample browser-hosted Void proxy, Minecraft server, and client.
- `docs` — documentation site and generated documentation assets.
- `pdk` — plugin development kit example.
- `src` — production code, sample apps, debug harnesses, benchmarks, and built-in plugins.
- `tests` — xUnit test projects and integration test infrastructure.

## Coding conventions

Follow the existing C# style rules:

- Indentation uses four spaces, with tab width set to four.
- Private fields prefixed with an underscore and method parameters use camelCase.
- Braces sit on their own lines when used, but never use braces for single-line statement bodies.
- Method signatures remain on a single line.
- Identifier names are fully descriptive and never abbreviated.
- Async methods carry an `Async` suffix.
- Pattern matching is preferred over explicit casts.
- `nameof` is used for parameter checks, logging, and exceptions.
- Never use the null-forgiving `!` operator to silence possible null reference warnings.

## Development

- Commit only hunks with actual code changes. All code should use CRLF line endings and existing whitespace should be preserved; never reformat untouched lines.
- Run `dotnet test tests/Void.UnitTests/` to ensure unit tests pass whenever C# source files (`*.cs`) are modified. Skip this step if no C# files change.
- Do not run integration tests (`tests/Void.IntegrationTests/`) unless explicitly asked to do so.
- Be patient with long-running tests and avoid aborting them early; some may take several minutes to complete.
- After changing source code, run `dotnet build` from the repository root.
- Conventional Commits are required for commit messages.
- Commit messages must include a scope after the type, e.g., `docs(readme): ...`.
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
- Commit bodies are required and must include a brief note about any observable behavior change.
- Use the `fix` or `feat` type only when your changes modify the proxy code in `./src`. For documentation, CI, or other unrelated updates, choose a more appropriate type such as `docs` or `chore`.
- Append a [gitmoji](https://gitmoji.dev/specification) after the commit scope, e.g., `feat(api): ✨ added new endpoint`.
- Use past tense for commit subjects when the change represents a completed action, e.g., `fixed bug` rather than `fix bug`.
- Pull request titles should follow the same Conventional Commits format.
- Pull request descriptions must use the following template:

  ```
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
- Never modify `CHANGELOG.md`; it is generated automatically by `release-please` from commit history.

## Documentation

- When adding links to documentation, make the link text bold. For example: `[**link text**](https://example.com)`.
- Never include inline code or backticks inside a link caption; keep code formatting outside the link text.

## Protocol guidance

When adding or modifying packet handling:

- Identify the Minecraft version whose packets require updates.
- Look up the release date of the next minor update (for example, if targeting 1.21, note that 1.21.1 released on August 8, 2024).
- Open the [Java Edition protocol/Packets](https://minecraft.wiki/w/Java_Edition_protocol/Packets?action=history&limit=500) page and view its revision history.
- Select the last revision made before that next update's release date (for 1.21, use the August 5, 2024 revision).
- Use that snapshot as your reference when altering the target version's protocol.
- For sub-versions, repeat this process using the next sub-version's release date and its preceding revision.
- Check the `src/Plugins/ProtocolSupport` directory to see how existing packets are implemented and update them as necessary.
- Base any protocol modifications on verified information from that official reference; avoid speculation.
- When communicating protocol-related changes, quote the relevant text and provide links to the exact protocol revision so reviewers can validate your reasoning.
