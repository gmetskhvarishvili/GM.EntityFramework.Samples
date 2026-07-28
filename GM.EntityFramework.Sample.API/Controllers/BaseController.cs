using GM.Mediator.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GM.EntityFramework.Sample.API.Controllers;

public class BaseController : ControllerBase
{
    protected IMediator Mediator =>
        field ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}
