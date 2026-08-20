using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DtoLayer.Dtos.Playlist
{
    public class CreatePlaylistDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }

    public class AddSongToPlaylistDto
    {
        public int SongId { get; set; }
    }
}
