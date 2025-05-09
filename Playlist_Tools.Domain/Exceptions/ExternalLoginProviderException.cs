namespace Playlist_Tools.Domain.Exceptions;

public class ExternalLoginProviderException(string provider, string message) :
    Exception($"External login provider : {provider} error occured : {message}");