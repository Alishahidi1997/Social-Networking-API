using System.Security.Claims;
using API.Models.Dto;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LikesController(ILikesService likesService) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> LikeUser(
        int targetUserId,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] LikeUserDto? body,
        CancellationToken ct)
    {
        return await likesService.AddLikeAsync(UserId, targetUserId, body?.PhotoId, ct) switch
        {
            LikeAddResult.Success or LikeAddResult.AlreadyLiked => Ok(),
            LikeAddResult.InvalidTarget => BadRequest("Cannot like yourself or user not found"),
            LikeAddResult.InvalidPhoto => BadRequest("That photo does not belong to this user"),
            LikeAddResult.DailyLimitReached => StatusCode(StatusCodes.Status429TooManyRequests,
                "You can send at most 20 likes per day (UTC)."),
            LikeAddResult.Failed => BadRequest("Could not save like"),
            _ => BadRequest()
        };
    }
}
