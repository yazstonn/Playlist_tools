using Playlist_Tools.Domain.Entities;

namespace Playlist_tools.Application.Abstracts;

public interface IAuthTokenProcessor
{
    (string jwtToken, DateTime expiresAt) GenerateJwtToken(User user);
    string GenerateRefreshToken();
    void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration);
}