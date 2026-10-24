using GM.EntityFramework.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GM.EntityFramework.Sample.Persistence.Context;

public class ApplicationDbContext: GenericDbContext
{
    public const string DefaultSchema = "application";

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // maps the domain-event log
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}