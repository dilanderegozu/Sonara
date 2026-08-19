using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DtoLayer.Dtos.Admin
{

    public class TopSongDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public long PlayCount { get; set; }
    }
}
