using System.Security.Claims;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MuteController(IUserModerationService moderationService) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("{userId:int}")]
    public async Task<ActionResult> Mute(int userId, CancellationToken ct)
    {
        if (!await moderationService.MuteAsync(UserId, userId, ct))
            return BadRequest("Cannot mute this user.");

        return NoContent();
    }

    [HttpDelete("{userId:int}")]
    public async Task<ActionResult> Unmute(int userId, CancellationToken ct)
    {
        if (!await moderationService.UnmuteAsync(UserId, userId, ct))
            return BadRequest("Not muting this user.");

        return NoContent();
    }
}
