using Playlist_Tools.Domain.Entities;

namespace Playlist_tools.Application.Abstracts;

public interface IUserRepository
{
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
}