using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Playlist_tools.Application.Abstracts;
using Playlist_Tools.Domain.Entities;
using Playlist_Tools.Domain.Exceptions;
using Playlist_Tools.Domain.Requests;

namespace Playlist_Tools.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAuthTokenProcessor _authTokenProcessor;
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;

    public AccountService(IAuthTokenProcessor authTokenProcessor, UserManager<User> userManager,
        IUserRepository userRepository)
    {
        _authTokenProcessor = authTokenProcessor;
        _userManager = userManager;
        _userRepository = userRepository;
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

    public async Task LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal)
    {

        if (claimsPrincipal == null)
        {
            throw new ExternalLoginProviderException("Google", "ClaimsPrincipal is null");
        }
        
        var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

        if (email == null)
        {
            throw new ExternalLoginProviderException("Google", "Email is null");
        }
        
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            var newUser = new User
            {
                UserName = email,
                Email = email,
                FirstName = claimsPrincipal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
                LastName = claimsPrincipal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
                EmailConfirmed = true

            };
            
            var result = await _userManager.CreateAsync(newUser);

            if (!result.Succeeded)
            {
                throw new ExternalLoginProviderException("Google", $"Failed to create user : {
                    string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            
            user = newUser;
        }
        
         var info = new UserLoginInfo("Google", claimsPrincipal.FindFirstValue(ClaimTypes.Email ?? string.Empty),"Google");
         
         var loginResult = await _userManager.AddLoginAsync(user, info);

         if (!loginResult.Succeeded)
         {
             throw new ExternalLoginProviderException("Google", $"Failed to login the user : {
                 string.Join(", ", loginResult.Errors.Select(e => e.Description))}");
         }

         await CreateJwtAndRefreshTokens(user);
    }
}