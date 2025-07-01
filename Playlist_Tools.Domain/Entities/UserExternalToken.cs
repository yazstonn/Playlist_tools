using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playlist_Tools.Domain.Entities
{
    public class UserExternalToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }

        public string Provider { get; set; } = "Google";
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }

        // Relation vers l'utilisateur
        public User User { get; set; }
    }
}
