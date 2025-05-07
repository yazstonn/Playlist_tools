namespace Playlist_Tools.Domain.Exceptions;

public class RegistrationFailedException(IEnumerable<string> errorDescription)
    : Exception($"Registration failed with following errors : {string.Join(Environment.NewLine, errorDescription)}");