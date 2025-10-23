using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using RotaLivreMaui.Models;

namespace RotaLivreMauiServices;

public class ApiService
{
    private readonly HttpClient _http;
    public ApiService(HttpClient http) => _http = http;

    public async Task<List<PasseioDto>> GetPasseiosAsync()
    {
        var resp = await _http.GetAsync("api/passeios");
        resp.EnsureSuccessStatusCode();
        var text = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PasseioDto>>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<PasseioDto>();
    }
}
