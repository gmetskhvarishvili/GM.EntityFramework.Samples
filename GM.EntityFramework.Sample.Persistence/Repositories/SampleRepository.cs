using GM.EntityFramework.Persistence.Repositories;
using GM.EntityFramework.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate;
using GM.EntityFramework.Sample.Persistence.Context;

namespace GM.EntityFramework.Sample.Persistence.Repositories;

public class SampleRepository(ApplicationDbContext context)
    : GenericRepository<GM.EntityFramework.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate.Sample,
        ApplicationDbContext>(context), ISampleRepository;