using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playlist_Tools.Application.Abstracts
{
    public interface IUserTokenRepository
    {
        Task SaveTokenAsync(Guid userId, string provider, string accessToken, DateTime expiresAt);
        Task<string?> GetTokenAsync(Guid userId, string provider);
    }
}
