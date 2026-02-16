using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Maui.Storage;

namespace RotaLivreMobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    private const string BaseUrl = "https://localhost:7015/api/";

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    public class LoginResponse
    {
        public string Token { get; set; }
    }

    public async Task<bool> Login(string email, string senha)
    {
        var loginData = new { email, senha };

        var json = JsonSerializer.Serialize(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("AuthApi/login", content);

        if (!response.IsSuccessStatusCode)
            return false;

        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<LoginResponse>(
            responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        await SecureStorage.SetAsync("auth_token", result.Token);

        return true;
    }

    private async Task AddAuthorizationHeader()
    {
        var token = await SecureStorage.GetAsync("auth_token");

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    // 🔐 Exemplo de GET autenticado
    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        await AddAuthorizationHeader();
        return await _httpClient.GetAsync(endpoint);
    }
}