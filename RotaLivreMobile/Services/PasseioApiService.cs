using System.Text.Json;

namespace RotaLivreMobile.Services;

public class PasseioApiService : BaseApiService
{
    public PasseioApiService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<PasseioDto?> GetByIdAsync(int id)
    {
        var response = await GetAsync($"PasseiosApi/{id}");

        if (response == null || !response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<PasseioDto>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
}