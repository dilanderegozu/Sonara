using Sonara.WebUI.Services;

namespace Sonara.WebUI.Models
{
    public class HomeViewModel
    {
        public string FirstName { get; set; }
        public int PlanLevel { get; set; }
        public int MaxPlanLevel { get; set; } = 1;
        public string PlanName { get; set; } = "Free";
        public List<RecentSongDto> RecentlyAdded { get; set; } = new();
        public List<PopularArtistDto> PopularArtists { get; set; } = new();
        public string Greeting { get; set; }
        public List<PlaylistDto> Playlists { get; set; } = new();
        public List<ContinueListeningDto> ContinueListening { get; set; } = new();
    }
}
