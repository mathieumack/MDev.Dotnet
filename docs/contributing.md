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

5. Run applicable tests if test projects are added or changed.
6. Open a pull request describing the behavior and validation.

The solution targets .NET 9 and .NET 10. Do not commit credentials; use configuration providers, environment variables, managed identity, or another secure secret store.

## Documentation

Package documentation belongs in `docs/packages/`. Use relative links for repository files and link platform guidance to [Microsoft Learn](https://learn.microsoft.com/). Verify every command, configuration property, namespace, and method name against the current source.

[Documentation home](index.md) · [Getting started](getting-started.md)
