using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Playlist_Tools.Domain.Entities;

namespace Playlist_Tools.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public new DbSet<User> Users { get; set; }

    public DbSet<UserExternalToken> ExternalTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>()
            .Property(u => u.FirstName).HasMaxLength(256);
        
        modelBuilder.Entity<User>()
            .Property(u => u.LastName).HasMaxLength(256);

        modelBuilder.Entity<UserExternalToken>()
       .HasOne(t => t.User)
       .WithMany()
       .HasForeignKey(t => t.UserId);
    }
}