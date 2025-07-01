using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Playlist_tools.Application.Abstracts;
using Playlist_Tools.Domain.Entities;
using Playlist_Tools.Domain.Requests;
using System.Security.Claims;

namespace Playlist_Tools.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<User> _signInManager;
        private readonly LinkGenerator _linkGenerator;

        public AccountController(IAccountService accountService, IHttpContextAccessor httpContextAccessor, SignInManager<User> signInManager, LinkGenerator linkGenerator)
        {
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _linkGenerator = linkGenerator;
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

        [HttpPost("login/google-token")]
        public async Task<IResult> LoginWithGoogleToken([FromBody] GoogleTokenRequest request)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
            }
            catch (Exception)
            {
                return Results.BadRequest("Invalid Google ID token.");
            }

            // Crée un utilisateur avec payload.Email ou connecte-le
            await _accountService.LoginWithGoogleTokenAsync(payload);

            // Retourne un JWT ou infos utilisateur
            return Results.Ok(new
            {
                Name = payload.Name,
                Email = payload.Email,
                Picture = payload.Picture
                // ...ton JWT si tu en génères
            });
        }

        [HttpGet("login/google")]
        public IResult LoginGoogle([FromQuery] string returnUrl)
        {
            var callbackUrl = _linkGenerator.GetPathByName(this.HttpContext, "GoogleLoginCallback")
                             + $"?returnUrl={Uri.EscapeDataString(returnUrl)}";

            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", callbackUrl);

            // Ajout des scopes OAuth nécessaires
            properties.Items["scope"] = "openid profile email https://www.googleapis.com/auth/youtube.readonly";

            return Results.Challenge(properties, ["Google"]);
        }

        [HttpGet("login/google/callback", Name = "GoogleLoginCallback")]
        public async Task<IResult> LoginGoogleCallback([FromQuery] string returnUrl)
        {
            var result = await _httpContextAccessor.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return Results.Unauthorized();

            var accessToken = result.Properties?.GetTokenValue("access_token");

            await _accountService.LoginWithGoogleAsync(result.Principal, accessToken);

            return Results.Redirect(returnUrl);
        }
    }
}
