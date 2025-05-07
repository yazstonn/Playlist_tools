namespace Playlist_Tools.Infrastructure.Options;

public class JwtOptions
{
    public const string JwtOptionsKey = "JwtOptions";
    
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Secret { get; set; }
    public int ExpirationTimeInMinutes { get; set; }
    
}