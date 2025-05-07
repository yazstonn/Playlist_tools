namespace Playlist_Tools.Domain.Exceptions;

public class LoginFailedException(string email) 
    : Exception($"Invalid email :{email} or password");