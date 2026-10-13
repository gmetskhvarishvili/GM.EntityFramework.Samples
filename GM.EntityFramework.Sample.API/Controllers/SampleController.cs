using GM.EntityFramework.Sample.Application.Samples.Commands.CreateSample;
using GM.EntityFramework.Sample.Application.Samples.Commands.DeleteSample;
using GM.EntityFramework.Sample.Application.Samples.Commands.UpdateSample;
using GM.EntityFramework.Sample.Application.Samples.Queries.GetSampleDetails;
using GM.EntityFramework.Sample.Application.Samples.Queries.GetSamplesList;
using Microsoft.AspNetCore.Mvc;

namespace GM.EntityFramework.Sample.API.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class SampleController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> AddSample([FromBody]CreateSampleCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSample([FromBody]UpdateSampleCommand command, CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);
        return Ok();
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteSample([FromQuery]DeleteSampleCommand command, CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);
        return Ok();
    }
    
    /// <summary>
    /// Get Samples List. Send <c>LastKeyProperty</c> + <c>LastKeyValue</c> for the samples after that row (in
    /// <c>OrderBy</c> order), or <c>CurrentPage</c> for
    /// that page by number. The body is the page's samples; paging metadata (<c>totalCount</c>,
    /// <c>pageSize</c>, <c>currentPage</c>, <c>totalPages</c>, <c>lastKeyProperty</c>, <c>lastKeyValue</c>,
    /// <c>hasMore</c>) is in the
    /// <c>X-Pagination</c> header.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SampleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSamplesList([FromQuery]GetSamplesListQuery query, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(query, cancellationToken);
        AddPaginationHeader(response.TotalCount, response.PageSize, response.CurrentPage, response.TotalPages,
            response.LastKeyProperty, response.LastKeyValue, response.HasMore);
        return Ok(response.Items);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetSampleDetails([FromQuery]GetSampleDetailsQuery query, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}