using System.Text.Json;

namespace RotaLivreMobile.Services;

public class PasseioApiService : BaseApiService
{
    public PasseioApiService(HttpClient httpClient) : base(httpClient)
    {
    }

    public class CurtidaResponse
    {
        public bool Curtiu { get; set; }
        public int TotalCurtidas { get; set; }
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

    public async Task<CurtidaResponse?> CurtirAsync(int id)
    {
        var response = await PostAsync($"PasseiosApi/{id}/curtir", null);

        if (response == null || !response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<CurtidaResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result ?? new CurtidaResponse();
    }
}