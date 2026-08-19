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

        public async Task<bool> PurchasePlanAsync(string jwtToken, int planId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/membership/purchase")
            {
                Content = JsonContent.Create(new { MembershipPlanId = planId })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

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
    }

    public class PopularArtistDto
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public long MonthlyListeners { get; set; }
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
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public int MaxDeviceCount { get; set; }
        public bool HasAds { get; set; }
        public bool HasOfflineDownload { get; set; }
        public bool HasHighQualityAudio { get; set; }
    }


}