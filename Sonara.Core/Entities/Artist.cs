using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.CoreLayer.Entities
{
    public class Artist
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Bio { get; set; }
        public long MonthlyListeners { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<Album> Albums { get; set; }
        public List<Song> Songs { get; set; }
    }
}
