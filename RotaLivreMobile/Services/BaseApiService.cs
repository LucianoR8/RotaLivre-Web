using System.Net;
using System.Net.Http.Headers;
using Microsoft.Maui.Storage;

namespace RotaLivreMobile.Services;

public class BaseApiService
{
    protected readonly HttpClient _httpClient;

    public BaseApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected async Task<bool> AddAuthorizationHeader()
    {
        var token = await SecureStorage.GetAsync("auth_token");

        if (string.IsNullOrEmpty(token))
        {
            await Shell.Current.GoToAsync("//LoginPage");
            return false;
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return true;
    }

    public async Task<HttpResponseMessage?> GetAsync(string endpoint)
    {
        var autorizado = await AddAuthorizationHeader();
        if (!autorizado) return null;

        var response = await _httpClient.GetAsync(endpoint);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            SecureStorage.Remove("auth_token");
            await Shell.Current.GoToAsync("//LoginPage");
            return null;
        }

        return response;
    }
}