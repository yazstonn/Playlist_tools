using System.Security.Claims;
using Google.Apis.Auth;
using Playlist_Tools.Domain.Entities;
using Playlist_Tools.Domain.Requests;

namespace Playlist_tools.Application.Abstracts;

public interface IAccountService
{
    Task RegisterUserAsync(RegisterRequest registerRequest);
    Task LoginAsync(LoginRequest loginRequest);
    Task RefreshTokenAsync(string? refreshToken);
    Task CreateJwtAndRefreshTokens(User user);
    Task LoginWithGoogleAsync(ClaimsPrincipal principal, string accessToken);
    Task LoginWithGoogleTokenAsync(GoogleJsonWebSignature.Payload payload);

}