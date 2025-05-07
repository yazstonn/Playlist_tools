// Dans le dossier Playlist_Tools.API/Controllers
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Playlist_Tools.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult GetMovies()
        {
            return Ok(new List<string>{"Matrix"});
        }
    }
}