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
    }

    public class LoginResultDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
    }
   
}