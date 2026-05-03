using RotaLivreMobile.Models;
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

    public async Task<bool> AlternarPendenteAsync(int id)
    {
        var response = await PostAsync($"PasseiosApi/{id}/pendente", null);

        if (response == null || !response.IsSuccessStatusCode)
            return false;

        var json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<Dictionary<string, bool>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result != null && result["pendente"];
    }

    public async Task<(List<PasseioDto> curtidos, List<PasseioDto> pendentes)> GetMeusPasseios()
    {
        var response = await GetAsync("PasseiosApi/meus");

        if (response == null || !response.IsSuccessStatusCode)
            return (new(), new());

        var json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<MeusPasseiosResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return (result?.Curtidos ?? new(), result?.Pendentes ?? new());
    }

    public async Task<bool> CadastrarUsuario(UsuarioCadastroDto dto)
    {
        var response = await PostAsync("Cadastrar_Usuario", dto);

        return response != null && response.IsSuccessStatusCode;
    }

    public async Task<MeusPasseiosResponse?> GetMeusPasseiosAsync()
    {
        var response = await GetAsync("api/PasseiosApi/meus");

        if (response == null || !response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<MeusPasseiosResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}