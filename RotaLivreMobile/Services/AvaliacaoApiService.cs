using System.Text.Json;
using RotaLivreMobile.Models;

namespace RotaLivreMobile.Services;

public class AvaliacaoApiService : BaseApiService
{
    public AvaliacaoApiService(HttpClient client)
        : base(client)
    {
    }

    public async Task<List<AvaliacaoDto>> ListarAsync(
        int idPasseio)
    {
        var response =
            await GetAsync($"AvaliacaoApi/{idPasseio}");

        if (response == null ||
            !response.IsSuccessStatusCode)
        {
            return new List<AvaliacaoDto>();
        }

        var json =
            await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<List<AvaliacaoDto>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<AvaliacaoDto>();
    }

    public async Task<bool> ComentarAsync(
        CriarAvaliacaoDto dto)
    {
        var response =
            await PostAsync(
                "AvaliacaoApi/comentar",
                dto);

        return response != null &&
               response.IsSuccessStatusCode;
    }
}