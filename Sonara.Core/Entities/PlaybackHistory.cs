using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.CoreLayer.Entities
{
    public class PlaybackHistory
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int SongId { get; set; }
        public Song Song { get; set; }

        public int PositionSeconds { get; set; }
        public DateTime LastPlayedAt { get; set; }
    }
}
