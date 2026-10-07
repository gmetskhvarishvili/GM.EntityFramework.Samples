using GM.EntityFramework.Persistence;
using GM.EntityFramework.Sample.Domain.SeedWork;
using GM.EntityFramework.Sample.Persistence.Context;
using GM.EntityFramework.Sample.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersistenceUnitOfWork = GM.EntityFramework.Sample.Persistence.UnitOfWork.UnitOfWork;

namespace GM.EntityFramework.Sample.Tests;

/// <summary>
/// Hosts a private SQLite in-memory database for a single test. Each call gets a fresh
/// <see cref="ApplicationDbContext"/> + unit of work over the same connection, mirroring how a
/// scoped DbContext works per web request (so create/update don't fight over change tracking).
/// </summary>
public sealed class SampleTestHost : IDisposable
{
    private readonly SqliteConnection _connection;

    // Empty provider: AddGMActorAuditing falls back to the anonymous actor + system clock, which is
    // all the sample needs to stamp CreatedAt/UpdatedAt in tests.
    private static readonly IServiceProvider AuditingServices = new ServiceCollection().BuildServiceProvider();

    public SampleTestHost()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    private ApplicationDbContext CreateContext()
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection);
        builder.AddGMActorAuditing(AuditingServices);
        return new ApplicationDbContext(builder.Options);
    }

    /// <summary>Runs a command/query handler in its own scope and returns the result.</summary>
    public async Task<TResult> ExecuteAsync<TResult>(Func<IUnitOfWork, Task<TResult>> action)
    {
        await using var context = CreateContext();
        var unitOfWork = new PersistenceUnitOfWork(context, new SampleRepository(context));
        return await action(unitOfWork);
    }

    /// <summary>Runs a command handler with no result in its own scope.</summary>
    public async Task ExecuteAsync(Func<IUnitOfWork, Task> action)
    {
        await using var context = CreateContext();
        var unitOfWork = new PersistenceUnitOfWork(context, new SampleRepository(context));
        await action(unitOfWork);
    }

    /// <summary>Reads directly from the database in its own scope (for assertions).</summary>
    public async Task<TResult> QueryAsync<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
    {
        await using var context = CreateContext();
        return await query(context);
    }

    public void Dispose() => _connection.Dispose();
}
