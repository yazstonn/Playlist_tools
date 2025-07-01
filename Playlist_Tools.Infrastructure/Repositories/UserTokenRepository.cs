using Microsoft.EntityFrameworkCore;
using Playlist_Tools.Domain.Entities;
using Playlist_Tools.Application.Abstracts;

namespace Playlist_Tools.Infrastructure.Repositories
{
    public class UserTokenRepository : IUserTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public UserTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveTokenAsync(Guid userId, string provider, string accessToken, DateTime expiresAt)
        {
            var existing = await _context.ExternalTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Provider == provider);

            if (existing != null)
            {
                existing.AccessToken = accessToken;
                existing.ExpiresAt = expiresAt;
            }
            else
            {
                await _context.ExternalTokens.AddAsync(new UserExternalToken
                {
                    UserId = userId,
                    Provider = provider,
                    AccessToken = accessToken,
                    ExpiresAt = expiresAt
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<string?> GetTokenAsync(Guid userId, string provider)
        {
            var token = await _context.ExternalTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Provider == provider);

            return token?.AccessToken;
        }
    }

}
