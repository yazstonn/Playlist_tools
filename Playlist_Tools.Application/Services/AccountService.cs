using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Playlist_tools.Application.Abstracts;
using Playlist_Tools.Domain.Entities;
using Playlist_Tools.Domain.Exceptions;
using Playlist_Tools.Domain.Requests;
using System.Security.Claims;

namespace Playlist_Tools.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAuthTokenProcessor _authTokenProcessor;
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly SignInManager<User> _signInManager;


    public AccountService(IAuthTokenProcessor authTokenProcessor, UserManager<User> userManager,
        IUserRepository userRepository, SignInManager<User> signInManager)
    {
        _authTokenProcessor = authTokenProcessor;
        _userManager = userManager;
        _userRepository = userRepository;
        _signInManager = signInManager;
    }

    public async Task RegisterUserAsync(RegisterRequest registerRequest)
    {
        var userExists = await _userManager.FindByEmailAsync(registerRequest.Email) != null;

        if (userExists)
        {
            throw new UserAlreadyExistsException(email : registerRequest.Email);
        }
        
        var user = User.Create(registerRequest.Email, registerRequest.FirstName, registerRequest.LastName);
        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, registerRequest.Password);
        
        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            throw new RegistrationFailedException(result.Errors.Select(e => e.Description));
        }
    }

    public async Task LoginAsync(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
        {
            throw new LoginFailedException(loginRequest.Email);
        }

        await CreateJwtAndRefreshTokens(user);
        
    }

    public async Task RefreshTokenAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new RefreshTokenException("Refresh token is missing");
        }
        
        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);

        if (user == null)
        {
            throw new RefreshTokenException("Unable to retrieve user from refresh token");
        }

        if (user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new RefreshTokenException("Refresh token has expired");
        }

        await CreateJwtAndRefreshTokens(user);
    }

    public async Task CreateJwtAndRefreshTokens(User user)
    {
        var (jwtToken, expirationDate) = _authTokenProcessor.GenerateJwtToken(user);
        var refreshTokenValue = _authTokenProcessor.GenerateRefreshToken();
        
        var refreshTokenExpirationDate = DateTime.UtcNow.AddDays(7);
        
        user.RefreshToken = refreshTokenValue;
        user.RefreshTokenExpiry = refreshTokenExpirationDate;

        await _userManager.UpdateAsync(user);
        
        _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN",jwtToken,expirationDate);
        _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN",user.RefreshToken,refreshTokenExpirationDate);
    }

    public async Task LoginWithGoogleAsync(ClaimsPrincipal principal, string accessToken)
    {
        if (principal == null)
        {
            throw new ExternalLoginProviderException("Google", "ClaimsPrincipal is null");
        }

        var email = principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
        {
            throw new ExternalLoginProviderException("Google", "Email is null");
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new User
            {
                UserName = email,
                Email = email,
                FirstName = principal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
                LastName = principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                throw new ExternalLoginProviderException("Google", $"Failed to create user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }
        }

        // Vérifie si l'utilisateur a déjà un login externe Google
        var userLogins = await _userManager.GetLoginsAsync(user);
        if (!userLogins.Any(l => l.LoginProvider == "Google"))
        {
            var loginInfo = new UserLoginInfo("Google", email, "Google");
            var loginResult = await _userManager.AddLoginAsync(user, loginInfo);

            if (!loginResult.Succeeded)
            {
                throw new ExternalLoginProviderException("Google", $"Failed to link Google login: {string.Join(", ", loginResult.Errors.Select(e => e.Description))}");
            }
        }

        // Tu peux ici stocker l'access token dans un champ ou un service externe :
        // await _tokenStorageService.SaveAccessToken(user.Id, accessToken);

        // Optionnel : connecter l'utilisateur (via cookie ou JWT)
        await _signInManager.SignInAsync(user, isPersistent: false);

        // Ou générer JWT (si tu es en mode stateless)
        await CreateJwtAndRefreshTokens(user);
    }


    public async Task LoginWithGoogleTokenAsync(GoogleJsonWebSignature.Payload payload)
    {
        var user = await _userManager.FindByEmailAsync(payload.Email);
        if (user == null)
        {
            user = new User
            {
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                UserName = payload.Email,
                Email = payload.Email,
                // autres champs utiles
            };
            await _userManager.CreateAsync(user);
        }


        await CreateJwtAndRefreshTokens(user);
    }

}