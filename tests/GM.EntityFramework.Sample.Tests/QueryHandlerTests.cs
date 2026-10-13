using GM.EntityFramework.Domain.Specifications;
using GM.EntityFramework.Sample.Application.Samples.Commands.CreateSample;
using GM.EntityFramework.Sample.Application.Samples.Commands.DeleteSample;
using GM.EntityFramework.Sample.Application.Samples.Queries.GetSampleDetails;
using GM.EntityFramework.Sample.Application.Samples.Queries.GetSamplesList;
using Xunit;

namespace GM.EntityFramework.Sample.Tests;

public class QueryHandlerTests
{
    private static Task<int> CreateAsync(SampleTestHost host, string name) =>
        host.ExecuteAsync(uow => new CreateSampleCommandHandler(uow).Handle(
            new CreateSampleCommand { Name = name, Description = $"{name} desc" }, CancellationToken.None));

    [Fact]
    public async Task GetDetails_returns_the_aggregate_with_its_items()
    {
        using var host = new SampleTestHost();
        var id = await host.ExecuteAsync(uow => new CreateSampleCommandHandler(uow).Handle(
            new CreateSampleCommand
            {
                Name = "Widget",
                Description = "d",
                SampleItems = [new CreateSampleItemCommand { Name = "part", Description = "p" }]
            }, CancellationToken.None));

        var details = await host.ExecuteAsync(uow => new GetSampleDetailsQueryHandler(uow).Handle(
            new GetSampleDetailsQuery { Id = id }, CancellationToken.None));

        Assert.Equal("Widget", details.Name);
        Assert.NotNull(details.SampleItems);
        Assert.Single(details.SampleItems);
    }

    [Fact]
    public async Task GetDetails_throws_when_the_sample_is_missing()
    {
        using var host = new SampleTestHost();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => host.ExecuteAsync(uow =>
            new GetSampleDetailsQueryHandler(uow).Handle(
                new GetSampleDetailsQuery { Id = 999 }, CancellationToken.None)));
    }

    [Fact]
    public async Task GetList_returns_only_visible_samples()
    {
        using var host = new SampleTestHost();
        await CreateAsync(host, "Visible");
        var doomedId = await CreateAsync(host, "Gone");

        await host.ExecuteAsync(uow => new DeleteSampleCommandHandler(uow).Handle(
            new DeleteSampleCommand { Id = doomedId }, CancellationToken.None));

        var list = await host.ExecuteAsync(uow => new GetSamplesListQueryHandler(uow).Handle(
            new GetSamplesListQuery(), CancellationToken.None));

        var sample = Assert.Single(list.Items);       // the soft-deleted one is filtered out
        Assert.Equal("Visible", sample.Name);
    }

    private static async Task<SampleTestHost> HostWithFiveVisibleSamplesAsync()
    {
        var host = new SampleTestHost();
        foreach (var name in new[] { "A", "B", "C", "D", "E" })
            await CreateAsync(host, name);
        var doomedId = await CreateAsync(host, "Gone");
        await host.ExecuteAsync(uow => new DeleteSampleCommandHandler(uow).Handle(
            new DeleteSampleCommand { Id = doomedId }, CancellationToken.None));
        return host;
    }

    private static Task<SamplesListDto> ListAsync(SampleTestHost host, GetSamplesListQuery query) =>
        host.ExecuteAsync(uow => new GetSamplesListQueryHandler(uow).Handle(query, CancellationToken.None));

    [Fact]
    public async Task GetList_by_current_page_reports_totals()
    {
        using var host = await HostWithFiveVisibleSamplesAsync();

        GetSamplesListQuery Page(int number) => new() { CurrentPage = number, PageSize = 2, OrderBy = "Id asc" };

        var second = await ListAsync(host, Page(2));
        var third = await ListAsync(host, Page(3));

        Assert.Equal(["C", "D"], second.Items.Select(s => s.Name));
        Assert.Equal(2, second.CurrentPage);
        Assert.Equal(5, second.TotalCount);           // soft-deleted "Gone" is not counted
        Assert.Equal(3, second.TotalPages);
        Assert.True(second.HasMore);
        Assert.Equal(["E"], third.Items.Select(s => s.Name));
        Assert.False(third.HasMore);
    }

    [Fact]
    public async Task GetList_by_last_key_value_walks_visible_samples()
    {
        using var host = await HostWithFiveVisibleSamplesAsync();

        GetSamplesListQuery After(string? lastKeyValue) => new() { LastKeyValue = lastKeyValue, PageSize = 2, OrderBy = "Id asc" };

        var first = await ListAsync(host, After(null));
        var second = await ListAsync(host, After(first.LastKeyValue));
        var third = await ListAsync(host, After(second.LastKeyValue));

        Assert.Equal(["A", "B"], first.Items.Select(s => s.Name));
        Assert.True(first.HasMore);
        Assert.Equal(5, first.TotalCount);            // counted in both paging modes
        Assert.Equal(["C", "D"], second.Items.Select(s => s.Name));
        Assert.Equal(["E"], third.Items.Select(s => s.Name)); // soft-deleted "Gone" is not paged in
        Assert.False(third.HasMore);
    }

    [Fact]
    public async Task GetList_by_last_key_value_with_id_desc_walks_newest_first()
    {
        using var host = await HostWithFiveVisibleSamplesAsync();

        GetSamplesListQuery Before(string? lastKeyValue) => new() { LastKeyValue = lastKeyValue, PageSize = 2, OrderBy = "Id desc" };

        var first = await ListAsync(host, Before(null));
        var second = await ListAsync(host, Before(first.LastKeyValue));
        var third = await ListAsync(host, Before(second.LastKeyValue));

        Assert.Equal(["E", "D"], first.Items.Select(s => s.Name));
        Assert.Equal(["C", "B"], second.Items.Select(s => s.Name));
        Assert.Equal(["A"], third.Items.Select(s => s.Name));
        Assert.False(third.HasMore);
    }

    [Fact]
    public async Task GetList_pages_by_the_key_property_the_caller_names()
    {
        using var host = await HostWithFiveVisibleSamplesAsync();

        var first = await ListAsync(host, new GetSamplesListQuery { LastKeyProperty = "Id", PageSize = 2, OrderBy = "Id asc" });
        var second = await ListAsync(host, new GetSamplesListQuery { LastKeyProperty = "Id", LastKeyValue = first.LastKeyValue, PageSize = 2 });

        Assert.Equal(["A", "B"], first.Items.Select(s => s.Name));
        Assert.Equal(first.Items[^1].Id.ToString(System.Globalization.CultureInfo.InvariantCulture), first.LastKeyValue); // LastKeyValue is the last item's LastKeyProperty value
        Assert.Equal(["C", "D"], second.Items.Select(s => s.Name));
    }

    [Fact]
    public async Task GetList_pages_by_a_string_key_property()
    {
        using var host = await HostWithFiveVisibleSamplesAsync();

        GetSamplesListQuery After(string? lastName) =>
            new() { LastKeyProperty = "Name", LastKeyValue = lastName, PageSize = 2, OrderBy = "Name asc" };

        var first = await ListAsync(host, After(null));
        var second = await ListAsync(host, After(first.LastKeyValue));

        Assert.Equal(["A", "B"], first.Items.Select(s => s.Name));
        Assert.Equal("B", first.LastKeyValue);       // the last item's Name, not its Id
        Assert.Equal(["C", "D"], second.Items.Select(s => s.Name));
    }

    [Fact]
    public async Task GetList_without_paging_parameters_returns_page_one_by_number()
    {
        using var host = await HostWithFiveVisibleSamplesAsync();

        var page = await ListAsync(host, new GetSamplesListQuery { PageSize = 2, OrderBy = "Id asc" });

        Assert.Equal(["A", "B"], page.Items.Select(s => s.Name));
        Assert.Equal(1, page.CurrentPage);   // paged by number, not by last key value
        Assert.Equal(3, page.TotalPages);
        Assert.True(page.HasMore);
    }

    [Fact]
    public async Task GetList_continues_after_the_named_row_in_the_requested_order()
    {
        using var host = await HostWithFiveVisibleSamplesAsync(); // A..E are Ids 1..5

        // "B" identifies the last row the client has (Id 2); ordered by Id descending, the next one is Id 1.
        var page = await ListAsync(host, new GetSamplesListQuery
        {
            LastKeyProperty = "Name", LastKeyValue = "B", OrderBy = "Id desc"
        });

        Assert.Equal(["A"], page.Items.Select(s => s.Name));
        Assert.Equal("Name", page.LastKeyProperty);
        Assert.Equal("A", page.LastKeyValue);
    }

    [Fact]
    public async Task GetList_rejects_a_last_key_value_that_does_not_fit_the_key_type()
    {
        using var host = await HostWithFiveVisibleSamplesAsync();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            ListAsync(host, new GetSamplesListQuery { LastKeyValue = "abc", PageSize = 2 })); // Id is an int
    }

    [Fact]
    public async Task GetList_rejects_an_unknown_key_property()
    {
        using var host = await HostWithFiveVisibleSamplesAsync();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            ListAsync(host, new GetSamplesListQuery { LastKeyProperty = "Nope", LastKeyValue = "1", PageSize = 2 }));
    }

    [Fact]
    public async Task GetList_skips_the_total_count_when_not_wanted()
    {
        using var host = await HostWithFiveVisibleSamplesAsync();

        var page = await ListAsync(host, new GetSamplesListQuery
        {
            CurrentPage = 1, PageSize = 2, OrderBy = "Id asc", IncludeTotalCount = false
        });

        Assert.Equal(["A", "B"], page.Items.Select(s => s.Name));
        Assert.Null(page.TotalCount);
        Assert.Null(page.TotalPages);
        Assert.Equal(1, page.CurrentPage);
        Assert.True(page.HasMore); // without a total, a full page means there may be more
    }

    [Fact]
    public async Task GetList_can_include_deleted_samples()
    {
        using var host = await HostWithFiveVisibleSamplesAsync();

        var all = await ListAsync(host, new GetSamplesListQuery { Visibility = VisibilityScope.All }); // SoftRemove also deactivates + hides

        Assert.Equal(6, all.Items.Count);
        Assert.Single(all.Items, s => s.Name == "Gone"); // the soft-deleted sample is included
    }
}
