using RotaLivreMobile.Models;
using System.Text.Json;
using RotaLivreMobile.Models;

namespace RotaLivreMobile.Services;

public class UsuarioApiService : BaseApiService
{
    public UsuarioApiService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<List<PerguntaSegurancaDto>> GetPerguntasAsync()
    {
        var response = await _httpClient.GetAsync("UsuarioApi/perguntas");

        if (!response.IsSuccessStatusCode)
            return new List<PerguntaSegurancaDto>();

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<List<PerguntaSegurancaDto>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<(bool sucesso, string erro)> CadastrarUsuario(UsuarioCadastroDto dto)
    {
        var response = await PostAsync("UsuarioApi/cadastrar", dto);

        if (response == null)
            return (false, "Erro de conexão");

        if (response.IsSuccessStatusCode)
            return (true, null);

        var erro = await response.Content.ReadAsStringAsync();

        return (false, erro);
    }

    public async Task<UsuarioPerfilDto?> GetPerfil(int id)
    {
        var response = await GetAsync($"UsuarioApi/perfil/{id}");

        if (response == null || !response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<UsuarioPerfilDto>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<bool> AtualizarPerfil(UsuarioPerfilDto dto)
    {
        var response = await PutAsync("UsuarioApi/editar", dto);

        return response != null && response.IsSuccessStatusCode;
    }

    public async Task<bool> DeletarConta(int id)
    {
        var response = await DeleteAsync($"UsuarioApi/deletar/{id}");

        return response != null && response.IsSuccessStatusCode;
    }


}