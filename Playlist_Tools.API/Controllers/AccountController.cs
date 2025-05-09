// Dans le dossier Playlist_Tools.API/Controllers

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Playlist_tools.Application.Abstracts;
using Playlist_Tools.Domain.Entities;
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
        public async Task<IResult> Register(RegisterRequest registerRequest)
        {
            await _accountService.RegisterUserAsync(registerRequest);
            return Results.Ok();
        }

        [HttpPost("login")]
        public async Task<IResult> Login(LoginRequest loginRequest)
        {
            await _accountService.LoginAsync(loginRequest);
            return Results.Ok();
        }

        [HttpPost("refresh")]
        public async Task<IResult> Refresh()
        {
            var refreshToken = Request.Cookies["REFRESH_TOKEN"];
            await _accountService.RefreshTokenAsync(refreshToken);
            return Results.Ok();
        }

        [HttpGet("login/google")]
        public async Task<IResult> LoginGoogle([FromQuery] string returnUrl, LinkGenerator linkGenerator,
            SignInManager<User> signInManager, HttpContext context)
        {
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google",
                linkGenerator.GetPathByName(context, "GoogleLoginCallback")
                + $"?returnUrl={Uri.EscapeDataString(returnUrl)}");

            return Results.Challenge(properties, ["Google"]);
    }

        [HttpGet("login/google/callback", Name = "GoogleLoginCallback")]
        public async Task<IResult> LoginGoogleCallback([FromQuery] string returnUrl, HttpContext context, IAccountService accountService)
        {
            var result = await context.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return Results.Unauthorized();
            }
            
            await accountService.LoginWithGoogleAsync(result.Principal);

            return Results.Redirect(returnUrl);
        }
    }
}