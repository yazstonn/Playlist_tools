using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playlist_Tools.Application.Abstracts
{
    public interface IYouTubeService
    {
        Task<object> GetLikedMusicAsync();
    }
}
