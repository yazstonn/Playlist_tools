using Azure.Core;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Configuration;
using Playlist_Tools.Application.Abstracts;
using Playlist_Tools.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;


public class YouTubeServicePerso : IYouTubeService
{
    private readonly string _apiKey;
    private readonly IUserTokenRepository _userTokenRepository;

    public YouTubeServicePerso(IUserTokenRepository userTokenRepository)
    {
        _userTokenRepository = userTokenRepository;
    }

    public async Task<object> GetLikedMusicAsync()
    {
        string token = await _userTokenRepository.GetTokenAsync(new Guid("8224a083-3520-4c82-a845-08ddb87d77ec"), "Google");
        var youtubeService = new YouTubeService(new BaseClientService.Initializer
        {
            HttpClientInitializer = new AccessTokenCredential(token),
            ApplicationName = "PlaylistTools"
        });

        var allItems = new List<object>();
        string nextPageToken = null;

        do
        {
            var request = youtubeService.PlaylistItems.List("snippet,contentDetails");
            request.PlaylistId = "LM";
            request.MaxResults = 50;
            request.PageToken = nextPageToken; // Utilisez le jeton de page pour la pagination

            var response = await request.ExecuteAsync();

            var result = response.Items.Select(item => new
            {
                Title = item.Snippet.Title,
                VideoId = item.Snippet.ResourceId.VideoId,
                ChannelTitle = item.Snippet.VideoOwnerChannelTitle,
                Thumbnail = item.Snippet.Thumbnails?.Medium?.Url
            }).ToList();

            allItems.AddRange(result);

            nextPageToken = response.NextPageToken; // Mettez à jour le jeton de page pour la prochaine itération

        } while (nextPageToken != null); // Continuez jusqu'à ce qu'il n'y ait plus de pages

        return allItems;
    }


    public async Task<List<string>> GetPlaylistItemsByNameAsync(string playlistName)
    {
        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = _apiKey,
            
        });

        // Rechercher la playlist par son nom
        var searchRequest = youtubeService.Playlists.List("snippet,contentDetails");
        searchRequest.Mine = true;
        searchRequest.MaxResults = 50; // On suppose qu'il n'y a qu'une seule playlist avec ce nom



        var searchResponse = await searchRequest.ExecuteAsync();

        // Récupérer les éléments de la playlist
        var playlistItemsRequest = youtubeService.PlaylistItems.List("snippet");
        //playlistItemsRequest.PlaylistId = playlistId;
        playlistItemsRequest.MaxResults = 50;

        var items = new List<string>();

        /*try
        {
            var response = await playlistItemsRequest.ExecuteAsync();

            foreach (var item in response.Items)
            {
                items.Add(item.Snippet.Title);
            }
        }
        catch (Google.GoogleApiException ex)
        {
            // Gérer les exceptions spécifiques à l'API Google
            Console.WriteLine($"Google API Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            // Gérer les autres exceptions
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }*/

        return items;
    }
}
