# Contributing

Questions, bug reports, and feature requests are welcome in [GitHub Issues](https://github.com/mathieumack/MDev.Dotnet/issues).

## Propose a change

1. Fork the repository and create a focused branch.
2. Make the smallest change that solves the issue.
3. Keep public API examples and configuration keys aligned with `src/`.
4. Build the solution:

   ```bash
   dotnet restore src/MDev.Dotnet.sln
   dotnet build src/MDev.Dotnet.sln --no-restore
   ```

5. Run the unit tests:

   ```bash
   dotnet test src/MDev.Dotnet.sln --no-build
   ```

6. Open a pull request describing the behavior and validation.

The solution targets .NET 10. Do not commit credentials; use configuration providers, environment variables, managed identity, or another secure secret store.

## Documentation

Package documentation belongs in `docs/packages/`. Use relative links for repository files and link platform guidance to [Microsoft Learn](https://learn.microsoft.com/). Verify every command, configuration property, namespace, and method name against the current source.

`SKILL.md` is generated from `docs/agent-guidance.md`, `docs/index.md`, `docs/getting-started.md`, and every package guide linked from the documentation index. When adding a package, configuration option, environment variable, infrastructure requirement, or public usage pattern, update those authoritative sources and regenerate the skill:

```powershell
pwsh scripts/Generate-Skill.ps1
pwsh scripts/Generate-Skill.ps1 -Check
```

Commit the regenerated file. The generator validates package coverage, required headings, repository links, code fences, unresolved placeholders, and content resembling credentials. CI also publishes a version-stamped `mdev-dotnet-skill-*` artifact; use `-SourceVersion` and `-OutputPath` to create the same form locally without replacing the repository copy.

Build the documentation locally with the same DocFX version used by GitHub Pages:

```bash
dotnet tool install --global docfx --version 2.78.5
docfx docs/docfx.json
```

[Documentation home](index.md) · [Getting started](getting-started.md)
