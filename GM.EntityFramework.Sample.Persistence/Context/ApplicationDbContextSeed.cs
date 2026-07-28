using Microsoft.Extensions.Logging;

namespace GM.EntityFramework.Sample.Persistence.Context;

public class ApplicationDbContextSeed
{
    public async Task SeedAsync(ApplicationDbContext context,
        ILogger<ApplicationDbContextSeed> logger, int? retry = 0)
    {
        var retryForAvailability = retry ?? 0;

        try
        {
            // Seed reference/demo data here.
        }
        catch (Exception ex)
        {
            if (retryForAvailability < 10)
            {
                retryForAvailability++;

                logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(ApplicationDbContext));

                await SeedAsync(context, logger, retryForAvailability);
            }
        }
    }
}