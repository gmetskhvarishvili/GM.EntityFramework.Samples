# GM.EntityFramework Samples

[![CI](https://github.com/gmetskhvarishvili/GM.EntityFramework.Samples/actions/workflows/ci.yml/badge.svg)](https://github.com/gmetskhvarishvili/GM.EntityFramework.Samples/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A layered **DDD + CQRS** ASP.NET Core Web API that shows how to build a real application on
**[GM.EntityFramework](https://www.nuget.org/packages/GM.EntityFramework)** (repository / unit of
work / specifications / auditing) with **[GM.Mediator](https://www.nuget.org/packages/GM.Mediator)**
dispatching commands and queries. Targets **.NET 10**, backed by **PostgreSQL**.

## What it demonstrates

- A **`Sample` aggregate root** (`SoftDeletableEntity<int>`, `IAggregateRoot`) with child
  `SampleItem` entities, private setters, factory methods (`Create`) and encapsulated mutations.
- A custom **`ISampleRepository : IGenericRepository<Sample>`** implemented by deriving from
  `GenericRepository<Sample, ApplicationDbContext>`.
- **`ApplicationDbContext : GenericDbContext`** — applies EF configurations from the assembly and
  gets automatic `CreatedAt` / `UpdatedAt` auditing for free.
- A **`UnitOfWork : GenericUnitOfWork<ApplicationDbContext>`** exposing the aggregate repository.
- **CQRS** with GM.Mediator: `CreateSample` / `UpdateSample` / `DeleteSample` commands and
  `GetSampleDetails` / `GetSamplesList` queries — the controllers just `Mediator.Send(...)`.
- The **specification pattern** (`GetSamplesList` builds a `BaseSpecification` with a visibility
  filter and optional criteria).
- **Soft delete** — `DeleteSample` calls `SoftRemove()`, and queries filter out inactive/hidden/
  deleted rows.

## Architecture

```
GM.EntityFramework.Sample.Domain/        # aggregate, entities, ISampleRepository, IUnitOfWork
GM.EntityFramework.Sample.Application/    # CQRS commands + queries (GM.Mediator handlers)
GM.EntityFramework.Sample.Persistence/    # ApplicationDbContext, configs, migrations, repo, UoW
GM.EntityFramework.Sample.API/            # controllers + composition root
tests/GM.EntityFramework.Sample.Tests/    # xUnit handler tests (SQLite in-memory)
```

Dependencies flow inward: Domain has no infra dependencies; Application depends on Domain;
Persistence implements Domain; the API wires everything together.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- **PostgreSQL** (or adjust the provider). A quick local instance:
  ```bash
  docker run --name gm-sample-db -e POSTGRES_PASSWORD=123456 -p 5432:5432 -d postgres
  ```

## Running

1. Set the connection string in
   [`GM.EntityFramework.Sample.API/appsettings.json`](GM.EntityFramework.Sample.API/appsettings.json)
   (or `appsettings.Local.json`) under `ConnectionStrings:ApplicationDatabase`.
2. Apply migrations:
   ```bash
   dotnet ef database update \
     --project GM.EntityFramework.Sample.Persistence \
     --startup-project GM.EntityFramework.Sample.API
   ```
3. Run:
   ```bash
   dotnet run --project GM.EntityFramework.Sample.API
   ```
   Then open the Swagger UI to try the endpoints.

### Endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| `POST` | `/Sample/AddSample` | Create a sample (+ optional items); returns the new id |
| `PUT` | `/Sample/UpdateSample` | Update name/description |
| `DELETE` | `/Sample/DeleteSample?Id=` | Soft-delete a sample |
| `GET` | `/Sample/GetSamplesList` | List samples (optional id/name/description filter) |
| `GET` | `/Sample/GetSampleDetails?Id=` | Get a sample with its items |

## Testing

```bash
dotnet test
```

Handler tests run the full application slice — GM.Mediator handler → `UnitOfWork` →
`GenericRepository` → `GenericDbContext` — against a **SQLite in-memory** database, so no
PostgreSQL is needed to run them.

## License

MIT — see [LICENSE](LICENSE).
