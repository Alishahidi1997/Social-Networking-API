using System.Security.Claims;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LikesController(ILikesService likesService) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> LikeUser(int targetUserId, CancellationToken ct)
    {
        return await likesService.AddLikeAsync(UserId, targetUserId, ct) switch
        {
            LikeAddResult.Success or LikeAddResult.AlreadyLiked => Ok(),
            LikeAddResult.InvalidTarget => BadRequest("Cannot like yourself or user not found"),
            LikeAddResult.DailyLimitReached => StatusCode(StatusCodes.Status429TooManyRequests,
                "You can send at most 20 likes per day (UTC)."),
            LikeAddResult.Failed => BadRequest("Could not save like"),
            _ => BadRequest()
        };
    }
}
