using GM.EntityFramework.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate;
using GM.EntityFramework.Sample.Domain.SeedWork;
using GM.Mediator.Contracts;

namespace GM.EntityFramework.Sample.Application.Samples.Commands.CreateSample;

public class CreateSampleCommand : IRequest<int>
{
    public required string Name { get; set; }
    public required string Description { get; set; }

    public IEnumerable<CreateSampleItemCommand>? SampleItems { get; set; }
}

public class CreateSampleItemCommand
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}

/// <summary>
/// Handles the creation of a new Sample aggregate with optional sample items.
/// </summary>
public class CreateSampleCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateSampleCommand, int>
{
    public async Task<int> Handle(CreateSampleCommand request, CancellationToken cancellationToken)
    {
        // Create the root Sample aggregate
        var sample = Domain.BoundedContext.SampleBoundedContext.SampleAggregate.Sample
            .Create(request.Name, request.Description);

        // Add child items if any
        if (request.SampleItems?.Any() == true)
        {
            var items = request.SampleItems
                .Select(item => SampleItem.Create(item.Name, item.Description))
                .ToArray();
            
            sample.AddSampleItems(items);
        }

        // Persist the aggregate
        await unitOfWork.SampleRepository.AddAsync(sample, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return sample.Id;
    }
}
