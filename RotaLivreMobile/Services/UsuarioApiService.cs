using RotaLivreMobile.Models;
using System.Text.Json;

namespace RotaLivreMobile.Services;

public class UsuarioApiService : BaseApiService
{
    public UsuarioApiService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<List<PerguntaSegurancaDto>> GetPerguntasAsync()
    {
        var response = await _httpClient.GetAsync("api/UsuarioApi/perguntas");

        if (!response.IsSuccessStatusCode)
            return new List<PerguntaSegurancaDto>();

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<List<PerguntaSegurancaDto>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<bool> CadastrarUsuario(UsuarioCadastroDto dto)
    {
        var response = await PostAsync("api/UsuarioApi/cadastrar", dto);

        return response != null && response.IsSuccessStatusCode;
    }
}