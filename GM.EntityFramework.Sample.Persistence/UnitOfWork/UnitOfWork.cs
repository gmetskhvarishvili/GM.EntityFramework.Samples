using GM.EntityFramework.Persistence;
using GM.EntityFramework.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate;
using GM.EntityFramework.Sample.Domain.SeedWork;
using GM.EntityFramework.Sample.Persistence.Context;

namespace GM.EntityFramework.Sample.Persistence.UnitOfWork;

public class UnitOfWork(
    ApplicationDbContext context, 
    ISampleRepository sampleRepository)
    : GenericUnitOfWork<ApplicationDbContext>(context), IUnitOfWork
{
    public ISampleRepository SampleRepository { get; } = sampleRepository;
}