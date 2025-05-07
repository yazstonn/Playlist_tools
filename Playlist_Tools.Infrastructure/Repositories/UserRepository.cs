using Microsoft.EntityFrameworkCore;
using Playlist_tools.Application.Abstracts;
using Playlist_Tools.Domain.Entities;

namespace Playlist_Tools.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
   private readonly ApplicationDbContext _dbContext;

   public UserRepository(ApplicationDbContext dbContext)
   {
      _dbContext = dbContext;
   }

   public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
   {
      var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
      
      return user;
   }
}