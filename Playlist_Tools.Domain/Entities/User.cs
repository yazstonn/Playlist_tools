using Microsoft.AspNetCore.Identity;

namespace Playlist_Tools.Domain.Entities;

public class User: IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    public static User Create(string email, string firstName, string lastName)
    {
        return new User
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            UserName = email
        };
    }

    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }
}