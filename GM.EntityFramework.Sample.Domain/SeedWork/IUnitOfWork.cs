using GM.EntityFramework.Domain.Repositories;
using GM.EntityFramework.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate;

namespace GM.EntityFramework.Sample.Domain.SeedWork;

public interface IUnitOfWork : IGenericUnitOfWork
{
    public ISampleRepository SampleRepository { get; }
}