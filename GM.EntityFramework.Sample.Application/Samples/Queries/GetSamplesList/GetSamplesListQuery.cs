using System.Globalization;
using System.Reflection;
using GM.EntityFramework.Domain.Specifications;
using GM.EntityFramework.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate.Specifications;
using GM.EntityFramework.Sample.Domain.SeedWork;
using GM.Mediator.Contracts;
using SampleEntity = GM.EntityFramework.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate.Sample;

namespace GM.EntityFramework.Sample.Application.Samples.Queries.GetSamplesList;

/// <summary>
/// Paged, filtered sample list. Send <see cref="LastKeyProperty"/> (<c>Id</c> by default) + <see cref="LastKeyValue"/>
/// to identify the last sample you have and get the ones after it in <see cref="OrderBy"/> order, or
/// <see cref="CurrentPage"/> to get that page by number. With neither, page 1 is returned.
/// </summary>
public class GetSamplesListQuery : IRequest<SamplesListDto>
{
    public const int DefaultPageSize = 25;
    public const int MaxPageSize = 100;

    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    /// <summary>1-based page number; page 1 when neither this nor <see cref="LastKeyValue"/> is sent. Ignored when <see cref="LastKeyValue"/> is sent.</summary>
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }

    /// <summary>The unique property that identifies the last sample (with <see cref="LastKeyValue"/>); defaults to <c>Id</c>.</summary>
    public string? LastKeyProperty { get; set; }

    /// <summary>
    /// The <see cref="LastKeyProperty"/> value of the last sample the client already has; the next
    /// page is the samples after it in <see cref="OrderBy"/> order. A string so it fits any key column — it is
    /// converted to the column's type (e.g.
    /// <c>42</c>, a Guid, or an ISO date such as <c>2026-10-14T10:00:00Z</c>).
    /// </summary>
    public string? LastKeyValue { get; set; }


    /// <summary>Whether to return the total count of matching samples. Defaults to true; send false to skip the count query.</summary>
    public bool IncludeTotalCount { get; set; } = true;

    /// <summary>
    /// Sort order. When paging by <see cref="LastKeyValue"/> it must be one column with an optional direction
    /// (e.g. <c>"Id desc"</c>; ties are broken by <see cref="LastKeyProperty"/>) and defaults to <see cref="LastKeyProperty"/>
    /// ascending; when paging by <see cref="CurrentPage"/> it defaults to newest first.
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Which normally-hidden samples the list should include. Defaults to <see cref="VisibilityScope.VisibleOnly"/>;
    /// admin/maintenance queries can widen it (e.g. <see cref="VisibilityScope.IncludeDeleted"/>).
    /// </summary>
    public VisibilityScope Visibility { get; set; } = VisibilityScope.VisibleOnly;

    // Inclusive audit-timestamp range filters, carried as strings and parsed by the specification
    // (see BaseSpecification.ApplyAuditDateRangeFilter). Format: "dd/MM/yyyy HH:mm:ss".
    public string? CreatedAtFrom { get; set; }
    public string? CreatedAtTo { get; set; }
    public string? UpdatedAtFrom { get; set; }
    public string? UpdatedAtTo { get; set; }
}

public class GetSamplesListQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSamplesListQuery, SamplesListDto>
{
    public async Task<SamplesListDto> Handle(GetSamplesListQuery request, CancellationToken cancellationToken)
    {
        var dateRange = new AuditDateRange
        {
            CreatedAtFrom = request.CreatedAtFrom,
            CreatedAtTo = request.CreatedAtTo,
            UpdatedAtFrom = request.UpdatedAtFrom,
            UpdatedAtTo = request.UpdatedAtTo,
        };

        // Page size defaults to 25 and is capped at 100 (a raw 0 would otherwise become Take(0) and return
        // no rows). CurrentPage and LastKeyValue pass through as sent — PagingOptions decides from them whether
        // to page by number or by last key value, and converts the string LastKeyValue to the key column's type.
        var paging = new PagingOptions<string>
        {
            CurrentPage = request.CurrentPage,
            LastKeyValue = request.LastKeyValue,
            LastKeyProperty = request.LastKeyProperty,
            PageSize = request.PageSize switch
            {
                < 1 => GetSamplesListQuery.DefaultPageSize,
                > GetSamplesListQuery.MaxPageSize => GetSamplesListQuery.MaxPageSize,
                var size => size
            },
            IncludeTotalCount = request.IncludeTotalCount,
        };

        int? totalCount = null;
        if (paging.IncludeTotalCount)
        {
            var countSpec = new SampleSpecification(request.Id, request.Name, request.Description,
                dateRange, PagingOptions.None, OrderingOptions.None, request.Visibility);
            totalCount = await unitOfWork.SampleRepository.CountAsync(countSpec, cancellationToken);
        }

        var spec = new SampleSpecification(request.Id, request.Name, request.Description,
            dateRange, paging, new OrderingOptions { OrderBy = request.OrderBy }, request.Visibility);
        var entities = await unitOfWork.SampleRepository.ListAsync(spec, cancellationToken);

        int? totalPages = totalCount is { } count ? (int)Math.Ceiling(count / (double)paging.PageSize) : null;
        int? currentPage = paging.IsKeysetPaging ? null : Math.Max(paging.CurrentPage, 1);
        var lastKeyProperty = string.IsNullOrWhiteSpace(request.LastKeyProperty)
            ? BaseSpecification<SampleEntity>.DefaultLastKeyProperty
            : request.LastKeyProperty.Trim();

        return new SamplesListDto
        {
            Items = entities.Select(ToDto).ToList(),
            PageSize = paging.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = currentPage,
            LastKeyProperty = lastKeyProperty,
            LastKeyValue = LastKeyOf(entities, lastKeyProperty),
            // By page number with a total: compare with the page count. Otherwise (by last key value, or the
            // count was skipped): a full page means there may be more.
            HasMore = currentPage is { } page && totalPages is { } pages
                ? page < pages
                : entities.Count == paging.PageSize,
        };
    }

    /// <summary>
    /// The key value (<paramref name="lastKeyProperty"/>, <c>Id</c> by default) of the last sample, as a
    /// string — what the client sends as <c>LastKeyValue</c> for the next page. Null for an empty page.
    /// </summary>
    private static string? LastKeyOf(List<SampleEntity> samples, string lastKeyProperty)
    {
        if (samples.Count == 0)
            return null;

        var property = typeof(SampleEntity).GetProperty(lastKeyProperty, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        return property?.GetValue(samples[^1]) switch
        {
            null => null,
            DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture), // round-trips exactly
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            var value => value.ToString(),
        };
    }

    private static SampleDto ToDto(SampleEntity sample) => new()
    {
        Id = sample.Id,
        Name = sample.Name,
        Description = sample.Description,
    };
}

public class SamplesListDto
{
    public required IReadOnlyList<SampleDto> Items { get; init; }
    public int PageSize { get; init; }
    /// <summary>All samples matching the filter; null when the request sent <c>IncludeTotalCount=false</c>.</summary>
    public int? TotalCount { get; init; }
    public int? TotalPages { get; init; }

    /// <summary>The 1-based page number; null when paging by last key value.</summary>
    public int? CurrentPage { get; init; }

    /// <summary>The property that identifies the last sample (<c>Id</c> unless the request named another) — send it as <c>LastKeyProperty</c>.</summary>
    public string LastKeyProperty { get; init; } = BaseSpecification<SampleEntity>.DefaultLastKeyProperty;

    /// <summary>The <see cref="LastKeyProperty"/> value of the last sample on this page — send it as <c>LastKeyValue</c> for the next page.</summary>
    public string? LastKeyValue { get; init; }

    public bool HasMore { get; init; }
}

public class SampleDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
