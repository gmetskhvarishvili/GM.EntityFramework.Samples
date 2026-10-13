using GM.EntityFramework.Domain.Specifications;

namespace GM.EntityFramework.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate.Specifications;

public class SampleSpecification : BaseSpecification<Sample>
{
    public SampleSpecification(int? id, string? name, string? description,
        AuditDateRange dateRange, PagingOptions paging, OrderingOptions ordering,
        VisibilityScope visibility = VisibilityScope.VisibleOnly)
    {
        AddVisibilityFilter(visibility);

        if (id is > 0)
        {
            AddCriteria(s => s.Id == id);
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            AddCriteria(s => s.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            AddCriteria(s => s.Description.Contains(description));
        }

        ApplyListQuery(dateRange, paging, ordering);
    }
}
