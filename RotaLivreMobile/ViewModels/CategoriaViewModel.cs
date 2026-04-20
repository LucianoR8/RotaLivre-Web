using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text.Json;
using RotaLivreMobile.Services;

namespace RotaLivreMobile.ViewModels
{
    public class CategoriaViewModel : BaseViewModel
    {
        private readonly BaseApiService _api;

        public ObservableCollection<PasseioDto> Passeios { get; set; } = new();

        public int CategoriaId
        {
            set
            {
                _ = CarregarPasseios(value);
            }
        }

        public CategoriaViewModel(BaseApiService api)
        {
            _api = api;
        }

        private async Task CarregarPasseios(int categoriaId)
        {
            var response = await _api.GetAsync($"passeios/categoria/{categoriaId}");

            if (response == null || !response.IsSuccessStatusCode)
                return;

            var json = await response.Content.ReadAsStringAsync();

            var lista = JsonSerializer.Deserialize<List<PasseioDto>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            Passeios.Clear();

            foreach (var item in lista)
                Passeios.Add(item);
        }
    }
}