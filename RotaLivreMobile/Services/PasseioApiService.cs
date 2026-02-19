using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace RotaLivreMobile.Services;

public class PasseioApiService
{
    private readonly HttpClient _httpClient;

    public PasseioApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://192.168.56.1:7015/")
        };
    }

    public async Task<PasseioDto> GetByIdAsync(int id)
    {
        var token = await SecureStorage.GetAsync("auth_token");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync($"api/PasseiosApi/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<PasseioDto>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
}