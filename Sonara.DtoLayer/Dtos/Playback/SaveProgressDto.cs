using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DtoLayer.Dtos.Playback
{
    public class SaveProgressDto
    {
        public int SongId { get; set; }
        public int PositionSeconds { get; set; }
    }
}
