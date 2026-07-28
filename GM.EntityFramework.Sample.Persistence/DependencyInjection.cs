using GM.EntityFramework.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate;
using GM.EntityFramework.Sample.Domain.SeedWork;
using GM.EntityFramework.Sample.Persistence.Context;
using GM.EntityFramework.Sample.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GM.EntityFramework.Sample.Persistence;

public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkNpgsql();

            services.AddDbContextPool<ApplicationDbContext>((serviceProvider, options) =>
            {
                options.UseNpgsql(configuration.GetConnectionString("ApplicationDatabase"),
                    o =>
                    {
                        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        o.CommandTimeout(60);
                    });
                options.UseInternalServiceProvider(serviceProvider);
            });

            services.AddTransient<ISampleRepository, SampleRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork.UnitOfWork>();

            return services;
        }
    }