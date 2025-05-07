namespace Playlist_Tools.Domain.Exceptions;

public class RefreshTokenException(string message) 
    : Exception(message);