using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DtoLayer.Dtos.Song
{
    public class UpdateSongDto { public string Title { get; set; } public int ArtistId { get; set; } }
    public class UpdateArtistDto { public string Name { get; set; } public string? Bio { get; set; } }
    public class UpdateMoodDto { public string Name { get; set; } public string ColorHex { get; set; } }
}
