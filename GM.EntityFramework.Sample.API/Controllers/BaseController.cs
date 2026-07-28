using GM.Mediator.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GM.EntityFramework.Sample.API.Controllers;

public class BaseController : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}