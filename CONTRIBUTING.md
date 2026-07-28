# Contributing

Thanks for taking a look! This is a **sample** repository that demonstrates
[GM.EntityFramework](https://github.com/gmetskhvarishvili/GM.EntityFramework) and
[GM.Mediator](https://github.com/gmetskhvarishvili/GM.Mediator) — it isn't a published package,
so there's no versioning or release process to worry about.

## Prerequisites

- **.NET 10 SDK**
- **PostgreSQL** to run the API (the tests use SQLite in-memory and need no database).

```bash
dotnet build -c Release
dotnet test  -c Release
```

## Workflow

1. Branch off `master`: `git switch -c fix/something`.
2. Make your change; keep the layering intact (Domain has no infrastructure dependencies,
   Persistence implements Domain, the API is the composition root).
3. Add or update tests under `tests/GM.EntityFramework.Sample.Tests` where it makes sense.
4. Open a pull request into `master`. CI (`build` + tests) must pass.

## Commit messages

[Conventional Commits](https://www.conventionalcommits.org/) are appreciated for readable
history (`feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:`), though this repo doesn't
release packages, so they don't drive any automation.

## Code style

Enforced by [`.editorconfig`](.editorconfig). Run `dotnet format` before pushing if unsure.
