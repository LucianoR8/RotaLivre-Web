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

        if (response == null)
        {
            Console.WriteLine("Response NULL");
            return null;
        }

        Console.WriteLine($"Status: {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro API: {erro}");
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"JSON: {json}");

        return JsonSerializer.Deserialize<PasseioDto>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
}