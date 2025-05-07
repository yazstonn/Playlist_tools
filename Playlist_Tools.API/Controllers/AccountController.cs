// Dans le dossier Playlist_Tools.API/Controllers
using Microsoft.AspNetCore.Mvc;
using Playlist_tools.Application.Abstracts;
using Playlist_Tools.Domain.Requests;

namespace Playlist_Tools.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            await _accountService.RegisterUserAsync(registerRequest);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            await _accountService.LoginAsync(loginRequest);
            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["REFRESH_TOKEN"];
            await _accountService.RefreshTokenAsync(refreshToken);
            return Ok();
        }
    }
}