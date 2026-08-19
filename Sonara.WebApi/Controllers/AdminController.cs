using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using Sonara.DtoLayer.Dtos.Admin;

namespace Sonara.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IDashboardDal _dashboardDal;

        public AdminController(IDashboardDal dashboardDal)
        {
            _dashboardDal = dashboardDal;
        }

        [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var summary = new DashboardSummaryDto
            {
                TodayRegistrations = await _dashboardDal.GetTodayRegistrationCountAsync(),
                TodayPurchases = await _dashboardDal.GetTodayPurchaseCountAsync(),
                TotalRevenue = await _dashboardDal.GetTotalRevenueAsync()
            };
            return Ok(summary);
        }
        
        [HttpGet("top-songs")]
        public async Task<IActionResult> GetTopSongs([FromQuery] int count = 10)
        {
            var songs = await _dashboardDal.GetTopPlayedSongsAsync(count);
            var result = songs.Select(s => new TopSongDto
            {
                SongId = s.SongId,
                Title = s.Title,
                PlayCount = s.PlayCount
            });
            return Ok(result);
        }

        [HttpGet("top-artists")]
        public async Task<IActionResult> GetTopArtists([FromQuery] int count = 10)
        {
            var artists = await _dashboardDal.GetTopArtistsAsync(count);

            var result = artists.Select(x => new TopArtistDto
            {
                ArtistId = x.Artist.ArtistId,
                Name = x.Artist.Name,
                TotalPlays = x.TotalPlays
            });

            return Ok(result);
        }
    }
}
