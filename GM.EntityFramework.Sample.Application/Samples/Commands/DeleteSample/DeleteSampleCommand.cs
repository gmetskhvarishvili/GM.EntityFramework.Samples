using GM.EntityFramework.Sample.Domain.SeedWork;
using GM.Mediator.Contracts;

namespace GM.EntityFramework.Sample.Application.Samples.Commands.DeleteSample;

public class DeleteSampleCommand : IRequest
{
    public int Id { get; set; }
}

public class DeleteSampleCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteSampleCommand>
{
    public async Task Handle(DeleteSampleCommand request, CancellationToken cancellationToken)
    {
        // the root Sample aggregate
        var sample = await unitOfWork.SampleRepository
            .FirstOrDefaultAsync(x => x.Id == request.Id
                                      && x.IsActive
                                      && !x.IsDeleted
                                      && !x.IsHidden,
                true,
                null,
                cancellationToken)
            ?? throw new KeyNotFoundException($"Sample '{request.Id}' was not found.");

        sample.SoftRemove();

        // Persist the aggregate
        unitOfWork.SampleRepository.Update(sample);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}