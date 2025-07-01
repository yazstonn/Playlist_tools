using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Playlist_Tools.Application.Abstracts;

[ApiController]
[Route("api/[controller]")]
public class YouTubeController : ControllerBase
{
    private readonly IYouTubeService _youTubeService;

    public YouTubeController(IYouTubeService youTubeService)
    {
        _youTubeService = youTubeService;
    }

    [HttpGet("youtube/liked")]
    public async Task<IActionResult> GetLikedMusic()
    {
        /*var userId = User.GetUserId(); // À adapter selon ta méthode d'identification
        var token = await tokenRepo.GetTokenAsync(userId, "Google");

        if (token == null)
            return Unauthorized();*/

        var likedTracks = await _youTubeService.GetLikedMusicAsync();

        return Ok(likedTracks);
    }
}
