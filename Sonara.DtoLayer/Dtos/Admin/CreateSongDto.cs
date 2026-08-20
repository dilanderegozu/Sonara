using Microsoft.AspNetCore.Http;
namespace Sonara.DtoLayer.Dtos.Admin
{
    public class CreateSongDto
    {
        public string Title { get; set; }
        public int ArtistId { get; set; }
        public int? AlbumId { get; set; }
        public IFormFile AudioFile { get; set; }
        public IFormFile? CoverFile { get; set; }
        public List<int> AllowedPlanIds { get; set; } = new();
    }
}
