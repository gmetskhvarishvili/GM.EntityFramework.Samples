using GM.EntityFramework.Sample.Domain.SeedWork;
using GM.Mediator.Contracts;

namespace GM.EntityFramework.Sample.Application.Samples.Commands.UpdateSample;

public class UpdateSampleCommand : IRequest
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}

/// <summary>
/// Handles the update of an existing Sample aggregate
/// </summary>
public class UpdateSampleCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateSampleCommand>
{
    public async Task Handle(UpdateSampleCommand request, CancellationToken cancellationToken)
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

        sample.Update(request.Name, request.Description);

        // Persist the aggregate
        unitOfWork.SampleRepository.Update(sample);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}