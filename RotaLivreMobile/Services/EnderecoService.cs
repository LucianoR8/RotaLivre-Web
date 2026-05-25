using System.Text.Json;

namespace RotaLivreMobile.Services
{
    public class GeofencingInfo
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int RaioMetros { get; set; }
    }

    public class EnderecoService
    {
        private readonly HttpClient _httpClient;

        public EnderecoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeofencingInfo?> GetGeofencingAsync(int idPasseio)
        {
            try
            {
                var response = await _httpClient.GetAsync($"endereco/passeio/{idPasseio}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<GeofencingInfo>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                return null;
            }
        }
    }
}