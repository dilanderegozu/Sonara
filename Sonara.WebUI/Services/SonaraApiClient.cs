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
        public async Task<List<MoodDto>?> GetMoodsAsync(string jwtToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/homefeed/moods");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<List<MoodDto>>();
        }

        public async Task<(bool Success, string? Error)> CreatePlaylistAsync(string jwtToken, string name, string? description)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/playlist")
            {
                Content = JsonContent.Create(new { Name = name, Description = description })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            return (response.IsSuccessStatusCode, response.IsSuccessStatusCode ? null : body);
        }

        public async Task<bool> AddSongToPlaylistAsync(string jwtToken, int playlistId, int songId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/playlist/{playlistId}/songs")
            {
                Content = JsonContent.Create(new { SongId = songId })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        public async Task<List<RecommendedSongDto>?> GetRecommendationsAsync(string jwtToken, int count = 5)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/homefeed/recommendations?count={count}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<List<RecommendedSongDto>>();
        }
        public async Task<(bool Success, int PlaylistId)> CreatePlaylistWithIdAsync(string jwtToken, string name, string? description)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/playlist")
            {
                Content = JsonContent.Create(new { Name = name, Description = description })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return (false, 0);

            var data = await response.Content.ReadFromJsonAsync<CreatePlaylistResultDto>();
            return (true, data?.PlaylistId ?? 0);
        }
        public async Task<List<LibraryPlaylistDto>?> GetAllPlaylistsAsync(string jwtToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/HomeFeed/all-playlists");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<List<LibraryPlaylistDto>>();
        }
        public async Task<PlaylistDetailDto?> GetPlaylistDetailAsync(string jwtToken, int playlistId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/playlist/{playlistId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<PlaylistDetailDto>();
        }

        public async Task<bool> RemoveSongFromPlaylistAsync(string jwtToken, int playlistId, int songId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/playlist/{playlistId}/songs/{songId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdatePlaylistCoverAsync(string jwtToken, int playlistId, Stream fileStream, string fileName, string contentType)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(fileStream), "coverFile", fileName);

            var request = new HttpRequestMessage(HttpMethod.Put, $"api/playlist/{playlistId}/cover")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        public async Task<List<AllSongDto>?> GetAllSongsAsync(string jwtToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/homefeed/all-songs");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<List<AllSongDto>>();
        }
        public async Task<ArtistSongsDto?> GetArtistSongsAsync(string jwtToken, int artistId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/homefeed/artists/{artistId}/songs");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<ArtistSongsDto>();
        }
        public async Task<MoodDetailDto?> GetMoodDetailAsync(string jwtToken, int moodId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/homefeed/moods/{moodId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<MoodDetailDto>();
        }
        public async Task<List<PopularArtistDto>?> GetAllArtistsAsync(string jwtToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/homefeed/all-artists");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<List<PopularArtistDto>>();
        }
    }
    public class MoodDetailDto
    {
        public int MoodId { get; set; }
        public string Name { get; set; }
        public string ColorHex { get; set; }
        public List<MoodSongDto> Songs { get; set; } = new();
    }

    public class MoodSongDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string? CoverImageUrl { get; set; }
    }
    public class AllSongDto
    {
        public int PlayCount { get; set; }
        public int ArtistId { get; set; }
        public int SongId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string? CoverImageUrl { get; set; }
    }

    public class ArtistSongsDto
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public List<ArtistSongItemDto> Songs { get; set; } = new();
    }

    public class ArtistSongItemDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public string? CoverImageUrl { get; set; }
    }
    public class PlaylistDetailDto
    {
        public string? CoverImageUrl { get; set; }
        public int PlaylistId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<PlaylistSongDto> Songs { get; set; } = new();
    }

    public class PlaylistSongDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Duration { get; set; }
    }
    public class LibraryPlaylistDto
    {
        public string? CoverImageUrl { get; set; }
        public int PlaylistId { get; set; }
        public string Name { get; set; }
        public int SongCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class CreatePlaylistResultDto
    {
        public int PlaylistId { get; set; }
        public string Name { get; set; }
    }
    public class RecommendedSongDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string? CoverImageUrl { get; set; }
    }
    public class MoodDto
    {
        public int MoodId { get; set; }
        public string Name { get; set; }
        public string ColorHex { get; set; }
        public int SongCount { get; set; }
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
        public string? CoverImageUrl { get; set; }
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