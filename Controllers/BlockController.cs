using System.Security.Claims;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BlockController(IUserModerationService moderationService) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("{userId:int}")]
    public async Task<ActionResult> Block(int userId, CancellationToken ct)
    {
        if (!await moderationService.BlockAsync(UserId, userId, ct))
            return BadRequest("Cannot block this user.");

        return NoContent();
    }

    [HttpDelete("{userId:int}")]
    public async Task<ActionResult> Unblock(int userId, CancellationToken ct)
    {
        if (!await moderationService.UnblockAsync(UserId, userId, ct))
            return BadRequest("Not blocking this user.");

        return NoContent();
    }
}
