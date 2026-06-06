# Getting Started
Before doing anything in this repository, open `/AGENTS.md`, read it completely, and follow it strictly.

This is mandatory for all tasks, including code changes, commit messages, pull request titles, and pull request descriptions.

Do not generate a commit message, pull request title, pull request description, or code change unless it satisfies `/AGENTS.md`.

# Commit messages
Commit type selection has priority and must not default to `refactor`.

Use `refactor` only when all of these are true:
- the change only restructures existing code
- there is no observable behavior change
- there is no bug fix
- there is no new capability
- there is no performance improvement
- there is no dependency, build, CI, test, documentation, or style-only change

Type priority:
1. `fix` — bug behavior was corrected in `./src`
2. `feat` — user-visible or API-visible capability was added in `./src`
3. `perf` — runtime, memory, allocation, IO, startup, or throughput behavior was improved
4. `deps` — dependencies changed
5. `test` — tests changed
6. `docs` — documentation changed
7. `ci` — CI workflow changed
8. `build` — build system, project files, packaging, or container publishing changed
9. `style` — formatting/style-only change with no logic change
10. `chore` — maintenance change that fits none of the above
11. `refactor` — internal restructuring only, last resort

Never use `refactor` for documentation, build, CI, dependency, test, formatting, config, or chore changes.

Commit format:
`type(scope): gitmoji subject`

Scope rules:
- scope must be lowercase
- scope must describe the area, component, package, or directory affected
- scope must not be a file name
- scope must not include file extensions
- use `docs`, `build`, `ci`, `tests`, `container`, `proxy`, `api`, or similar logical scopes instead of names like `README.md`, `Program.cs`, or `.csproj`

Commit subjects must be written in past tense when describing completed work:
- `fix(proxy): 🐛 fixed scoped request context`
- `docs(readme): 📝 updated setup instructions`
- `build(container): 🏗️ added multi-architecture publishing`

Commit body is required and must include:
`Observable behavior: ...`

For pure refactors, write:
`Observable behavior: none.`
