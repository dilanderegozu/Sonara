namespace Sonara.CoreLayer.Entities
{
    public class Album
    {
        public int AlbumId { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? CoverImageUrl { get; set; }

        public int ArtistId { get; set; }
        public Artist Artist { get; set; }

        public ICollection<Song> Songs { get; set; }
    }
}