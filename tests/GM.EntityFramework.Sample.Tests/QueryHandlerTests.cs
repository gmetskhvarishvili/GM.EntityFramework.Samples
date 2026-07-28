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
        Assert.Single(details.SampleItems!);
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

        var samples = list.ToList();
        Assert.Single(samples);                       // the soft-deleted one is filtered out
        Assert.Equal("Visible", samples[0].Name);
    }
}
