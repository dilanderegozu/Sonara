using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.CoreLayer.Entities
{
    public class Mood
    {
        public int MoodId { get; set; }
        public string Name { get; set; }     
        public string ColorHex { get; set; }     

        public ICollection<SongMood> Songs { get; set; }
    }

    public class SongMood
    {
        public int SongId { get; set; }
        public Song Song { get; set; }

        public int MoodId { get; set; }
        public Mood Mood { get; set; }
    }
}
