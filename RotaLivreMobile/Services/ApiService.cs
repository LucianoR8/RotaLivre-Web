using Microsoft.Maui.Storage;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RotaLivreMobile.Models;

namespace RotaLivreMobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public class LoginResponse
    {
        public string Token { get; set; }
        public UsuarioResponse Usuario { get; set; }
    }

    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
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

        if (result == null)
            return false;

        await SecureStorage.SetAsync("auth_token", result.Token);
        await SecureStorage.SetAsync("usuario_id", result.Usuario.Id.ToString());
        await SecureStorage.SetAsync("usuario_nome", result.Usuario.Nome);

        return true;
    }

    private async Task AddAuthorizationHeader()
    {
        var token = await SecureStorage.GetAsync("auth_token");

        if (string.IsNullOrEmpty(token))
        {
            throw new Exception("TOKEN NÃO ENCONTRADO NO SECURE STORAGE");
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var tokenDebug = _httpClient.DefaultRequestHeaders.Authorization?.Parameter;

        // await Application.Current.MainPage.DisplayAlert("Token enviado", tokenDebug ?? "NULL", "OK");
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        await AddAuthorizationHeader();
        return await _httpClient.GetAsync(endpoint);
    }

    public async Task<int> GetUsuarioId()
    {
        var id = await SecureStorage.GetAsync("usuario_id");
        return int.TryParse(id, out var parsed) ? parsed : 0;
    }

    public async Task<string> GetNomeUsuario()
    {
        return await SecureStorage.GetAsync("usuario_nome");
    }

    public async Task<HomeDto> GetHome()
    {
        await AddAuthorizationHeader();

        var response = await _httpClient.GetAsync("HomeApi");

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            SecureStorage.Remove("auth_token");
            await Shell.Current.GoToAsync("//LoginPage");
            return null;
        }

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<HomeDto>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<GrupoResponseDto> CriarGrupo(int passeioId)
    {
        await AddAuthorizationHeader();

        var response = await _httpClient.PostAsync($"grupo/criar?passeioId={passeioId}", null);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<GrupoResponseDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public async Task<bool> GrupoExisteEAtivo(string codigo)
    {
        var response = await _httpClient.GetAsync($"grupo/validar?codigo={codigo}");

        if (!response.IsSuccessStatusCode)
            return false;

        var json = await response.Content.ReadAsStringAsync();

        return bool.Parse(json);
    }

}