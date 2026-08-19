using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DtoLayer.Dtos.Admin
{
    public class TopArtistDto
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public int TotalPlays { get; set; }
    }
}
