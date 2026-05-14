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

    public async Task<(bool sucesso, string erro)> AtualizarPerfil(UsuarioPerfilDto dto)
    {
        var response = await PutAsync("UsuarioApi/editar", dto);

        if (response == null)
            return (false, "Erro de conexão");

        if (response.IsSuccessStatusCode)
            return (true, null);

        var erro = await response.Content.ReadAsStringAsync();
        return (false, erro);
    }

    public async Task<bool> DeletarConta(int id)
    {
        var response = await DeleteAsync($"UsuarioApi/deletar/{id}");

        return response != null && response.IsSuccessStatusCode;
    }

    public async Task<string?> BuscarPerguntaSeguranca(string email)
    {
        try
        {
            Console.WriteLine("1 - Iniciando busca");

            var response = await GetAsync(
                $"UsuarioApi/pergunta?email={Uri.EscapeDataString(email)}");

            Console.WriteLine($"2 - Status: {response?.StatusCode}");

            if (response == null)
            {
                Console.WriteLine("3 - Response null");
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("4 - Status não sucesso");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"5 - JSON: {json}");

            var resultado = JsonSerializer.Deserialize<PerguntaResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            Console.WriteLine($"6 - Pergunta: {resultado?.Pergunta}");

            return resultado?.Pergunta;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO REAL: {ex}");

            return null;
        }
    }

    public async Task<bool> VerificarRespostaAsync(
    string email,
    string resposta)
    {
        var dto = new VerificarRespostaDto
        {
            Email = email,
            Resposta = resposta
        };

        var response =
            await PostAsync(
                "UsuarioApi/verificar-resposta",
                dto);

        return response != null &&
               response.IsSuccessStatusCode;
    }

    public async Task<bool> RedefinirSenhaAsync(
    string email,
    string novaSenha)
    {
        var dto = new RedefinirSenhaDto
        {
            Email = email,
            NovaSenha = novaSenha
        };

        var response =
            await PostAsync(
                "UsuarioApi/redefinir",
                dto);

        return response != null &&
               response.IsSuccessStatusCode;
    }
}