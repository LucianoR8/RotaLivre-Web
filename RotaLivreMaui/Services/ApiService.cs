using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RotaLivreMaui.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://192.168.15.46:7015/api/passeios"; // <-- altere o IP do PC

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<PasseioModel>> GetPasseiosAsync()
        {
            var url = $"{BaseUrl}passeios";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<PasseioModel>();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<PasseioModel>>(json, options) ?? new List<PasseioModel>();
        }
    }

    // Modelo simplificado compatível com o retorno da sua API
    public class PasseioModel
    {
        public int id_passeio { get; set; }
        public string nome_passeio { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}
