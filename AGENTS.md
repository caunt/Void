# Repository Guidelines

This repository is a .NET solution structured as follows:

- `src/` – All production source code.
  - `Api/` – API abstractions.
  - `Benchmarks/` – Benchmark projects.
  - `Debug/` – Debug harness and utilities.
  - `Minecraft/` – Minecraft specific libraries.
  - `Platform/` – Main proxy entry point.
  - `Playground/` – Sample playground app and documentation generator.
  - `Plugins/` – Built‑in plugins organized by feature.
  - `Servers/` – Integrations with server implementations such as Bukkit.
  - `Terminal/` – Terminal UI for interacting with the proxy.
- `tests/` – Unit test projects.
- `docs/` – Documentation site built with Astro.
- `pdk/` – Plugin development kit example project.
- `Void.slnx` – Solution file referencing all projects.

## Development

- Run `dotnet format` before committing any code changes.
- After changing source code, run `dotnet build` from the repository root.
- Conventional Commits are required for commit messages.
