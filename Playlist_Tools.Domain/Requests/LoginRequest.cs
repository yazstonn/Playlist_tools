namespace Playlist_Tools.Domain.Requests;

public class LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}