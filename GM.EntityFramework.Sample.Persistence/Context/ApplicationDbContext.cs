using GM.EntityFramework.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GM.EntityFramework.Sample.Persistence.Context;

public class ApplicationDbContext: GenericDbContext
{
    public const string DEFAULT_SCHEMA = "application";

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}