using GM.EntityFramework.Domain.Specifications;
using GM.EntityFramework.Sample.Domain.SeedWork;
using GM.Mediator.Contracts;

namespace GM.EntityFramework.Sample.Application.Samples.Queries.GetSamplesList;

public class GetSamplesListQuery : IRequest<IEnumerable<SampleModel>>
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class GetSamplesListQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSamplesListQuery, IEnumerable<SampleModel>>
{
    public async Task<IEnumerable<SampleModel>> Handle(GetSamplesListQuery request, CancellationToken cancellationToken)
    {
        var spec = new SampleSpecification(request.Id, request.Name, request.Description);
        var samples = await unitOfWork.SampleRepository.ListAsync(spec, cancellationToken);

        return samples.Select(sample => new SampleModel
        {
            Id = sample.Id,
            Name = sample.Name,
            Description = sample.Description
        }).ToList();
    }
}

public class SampleModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}

public class
    SampleSpecification : BaseSpecification<Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate.Sample>
{
    public SampleSpecification(int? id, string? name, string? description)
    {
        AddVisibilityFilter();
        
        if (id.HasValue)
            AddCriteria(s => s.Id == id.Value);

        if (!string.IsNullOrWhiteSpace(name))
            AddCriteria(s => s.Name!.Contains(name));

        if (!string.IsNullOrWhiteSpace(description))
            AddCriteria(s => s.Description!.Contains(description));
        
        
    }
}