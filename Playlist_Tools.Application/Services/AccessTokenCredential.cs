using Google.Apis.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Playlist_Tools.Application.Services
{
    public class AccessTokenCredential : IConfigurableHttpClientInitializer
    {
        private readonly string _accessToken;

        public AccessTokenCredential(string accessToken)
        {
            _accessToken = accessToken;
        }

        public void Initialize(ConfigurableHttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }
    }
}
