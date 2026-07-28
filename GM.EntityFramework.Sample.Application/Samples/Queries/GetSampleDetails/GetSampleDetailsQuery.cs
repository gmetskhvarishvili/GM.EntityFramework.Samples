using GM.EntityFramework.Sample.Domain.SeedWork;
using GM.Mediator.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GM.EntityFramework.Sample.Application.Samples.Queries.GetSampleDetails;

public class GetSampleDetailsQuery : IRequest<SampleDetailsModel>
{
    public int Id { get; set; }
}

public class GetSampleDetailsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSampleDetailsQuery, SampleDetailsModel>
{
    public async Task<SampleDetailsModel> Handle(GetSampleDetailsQuery request, CancellationToken cancellationToken)
    {
        var sample = await unitOfWork.SampleRepository
            .FirstOrDefaultAsync(x => x.Id == request.Id
                                      && x.IsActive
                                      && !x.IsDeleted
                                      && !x.IsHidden,
                false,
                x => x
                    .Include(o => o.SampleItems),
                cancellationToken)
            ?? throw new KeyNotFoundException($"Sample '{request.Id}' was not found.");

        var result = new SampleDetailsModel
        {
            Id = sample.Id,
            Name = sample.Name,
            Description = sample.Description
        };

        var items = sample
            .SampleItems
            .Select(sampleItem =>
                new SampleItemModel
                {
                    Id = sampleItem.Id, 
                    Name = sampleItem.Name, 
                    Description = sampleItem.Description
                }).ToList();
        result.SampleItems = items;

        return result;
    }
}

public class SampleDetailsModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    public IEnumerable<SampleItemModel>? SampleItems { get; set; }
}

public class SampleItemModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}