using System.Text;
using System.Text.Json;

namespace RotaLivreMobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    // ⚠ Se for Android Emulator use 10.0.2.2
    // ⚠ Se for Windows use https://localhost:PORTA
    private const string BaseUrl = "https://localhost:7015/api/";

    public ApiService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<bool> Login(string email, string senha)
    {
        var loginData = new
        {
            email = email,
            senha = senha
        };

        var json = JsonSerializer.Serialize(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{BaseUrl}AuthApi/login", content);

        return response.IsSuccessStatusCode;
    }
}