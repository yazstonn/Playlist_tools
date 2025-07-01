using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playlist_Tools.Domain.Requests
{
    public class GoogleTokenRequest
    {
        public required string IdToken { get; set; }
    }
}
