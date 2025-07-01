using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Playlist_Tools.Controllers
{
    public class YoutubeController : Controller
    {

        [HttpGet("youtube/playlists")]
        public async Task<IActionResult> GetPlaylists([FromServices] IHttpContextAccessor httpContextAccessor)
        {
            var accessToken = httpContextAccessor.HttpContext.User.FindFirst("YouTubeAccessToken")?.Value;

            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized("No access token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync("https://www.googleapis.com/youtube/v3/playlists?part=snippet&mine=true");
            if (!response.IsSuccessStatusCode)
                return BadRequest("Failed to fetch playlists");

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

    }
}
