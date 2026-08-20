using System.Net.Http.Headers;

namespace Sonara.WebUI.Services
{
    public class SonaraApiClient
    {
        private readonly HttpClient _httpClient;

        public SonaraApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SonaraApi");
        }

        public async Task<LoginResultDto?> LoginAsync(string email, string password, string deviceIdentifier)
        {
            var payload = new
            {
                Email = email,
                Password = password,
                DeviceIdentifier = deviceIdentifier
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", payload);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<LoginResultDto>();
        }
        public async Task<List<RecentSongDto>?> GetRecentlyAddedAsync(string jwtToken, int count = 8)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/homefeed/recently-added?count={count}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<List<RecentSongDto>>();
        }
        public async Task<List<PopularArtistDto>?> GetPopularArtistsAsync(string jwtToken, int count = 6)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/homefeed/popular-artists?count={count}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<List<PopularArtistDto>>();
        }
        public async Task<LoginResultDto?> RegisterAsync(string firstName, string lastName, string email, string password)
        {
            var payload = new
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/register", payload);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<LoginResultDto>();
        }
        public async Task<MyMembershipDto?> GetMyMembershipAsync(string jwtToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/membership/my-membership");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<MyMembershipDto>();
        }
        public async Task<List<PlanDto>?> GetPlansAsync(string jwtToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/membership/plans");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<List<PlanDto>>();
        }
        public async Task<(bool Success, string? Error)> PurchasePlanAsync(string jwtToken, int planId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/membership/purchase")
            {
                Content = JsonContent.Create(new { MembershipPlanId = planId })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            return (response.IsSuccessStatusCode, response.IsSuccessStatusCode ? null : body);
        }
        public async Task<List<PlaylistDto>?> GetMyPlaylistsAsync(string jwtToken, int count = 6)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/homefeed/my-playlists?count={count}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<List<PlaylistDto>>();
        }
        public async Task<List<ContinueListeningDto>?> GetContinueListeningAsync(string jwtToken, int count = 4)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/homefeed/continue-listening?count={count}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<List<ContinueListeningDto>>();
        }
        public async Task<(bool Success, PlaySongResultDto? Data, string? Error)> PlaySongAsync(string jwtToken, int songId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/song/{songId}/play");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return (false, null, body);

            var data = System.Text.Json.JsonSerializer.Deserialize<PlaySongResultDto>(body,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return (true, data, null);
        }
        public async Task SaveProgressAsync(string jwtToken, int songId, int positionSeconds)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/homefeed/playback-progress")
            {
                Content = JsonContent.Create(new { SongId = songId, PositionSeconds = positionSeconds })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            await _httpClient.SendAsync(request);
        }
    }
    public class PlaySongResultDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public string AudioUrl { get; set; }
    }
    public class ContinueListeningDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public int PositionSeconds { get; set; }
        public int TotalSeconds { get; set; }
        public int ProgressPercent { get; set; }
        public string? CoverImageUrl { get; set; }
    }
    public class PlaylistDto
    {
        public int PlaylistId { get; set; }
        public string Name { get; set; }
        public int SongCount { get; set; }
    }
    public class LoginResultDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
    }
    public class RecentSongDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string? CoverImageUrl { get; set; }
    }

    public class PopularArtistDto
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public long MonthlyListeners { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class MyMembershipDto
    {
        public string PlanName { get; set; }
        public int Level { get; set; }
        public int MaxLevel { get; set; }

    }
    public class PlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }   
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public int MaxDeviceCount { get; set; }
        public bool HasAds { get; set; }
        public bool HasOfflineDownload { get; set; }
        public bool HasHighQualityAudio { get; set; }
    }


}