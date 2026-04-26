using System.Security.Claims;
using API.Models.Dto;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/feed")]
[Authorize]
public class FeedController(IPostService postService) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("home")]
    public async Task<ActionResult<PagedResultDto<PostDto>>> GetHomeTimeline(
        [FromQuery(Name = "page")] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default) =>
        Ok(await postService.GetHomeTimelineAsync(UserId, page, pageSize, ct));
}
