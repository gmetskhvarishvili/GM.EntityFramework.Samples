using System.Text.Json;
using GM.Mediator.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GM.EntityFramework.Sample.API.Controllers;

public class BaseController : ControllerBase
{
    protected IMediator Mediator =>
        field ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    /// <summary>
    /// Writes a page's metadata to the <c>X-Pagination</c> response header so the body can be just the
    /// items. <paramref name="lastKeyProperty"/> + <paramref name="lastKeyValue"/> are what the client sends as
    /// <c>LastKeyProperty</c> / <c>LastKeyValue</c> for the next page;
    /// <paramref name="currentPage"/> is null when paging by last key value; <paramref name="totalCount"/> and
    /// <paramref name="totalPages"/> are null when the client asked to skip the count.
    /// </summary>
    protected void AddPaginationHeader(long? totalCount, long pageSize, long? currentPage, long? totalPages,
        string? lastKeyProperty, string? lastKeyValue, bool hasMore)
    {
        var metadata = new
        {
            totalCount,
            pageSize,
            currentPage,
            totalPages,
            lastKeyProperty,
            lastKeyValue,
            hasMore
        };

        Response.Headers["X-Pagination"] = JsonSerializer.Serialize(metadata);
    }
}
