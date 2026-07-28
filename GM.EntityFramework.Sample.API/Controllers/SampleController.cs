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
    
    [HttpGet]
    public async Task<IActionResult> GetSamplesList([FromQuery]GetSamplesListQuery query, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetSampleDetails([FromQuery]GetSampleDetailsQuery query, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}