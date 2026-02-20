using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RotaLivreMobile.ViewModels
{
    public class CategoriaViewModel : BaseViewModel
    {
        private int _categoriaId;

        public int CategoriaId
        {
            get => _categoriaId;
            set
            {
                _categoriaId = value;
                CarregarPasseios();
            }
        }

        public ObservableCollection<PasseioDto> Passeios { get; set; }

        public CategoriaViewModel()
        {
            Passeios = new ObservableCollection<PasseioDto>();
        }

        private async void CarregarPasseios()
        {
            try
            {
                using var client = new HttpClient();

                // Se você usa JWT salvo em Preferences:
                var token = Preferences.Get("jwt_token", string.Empty);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(
                    $"http://192.168.15.121:7015/api/passeios/categoria/{_categoriaId}");

                if (!response.IsSuccessStatusCode)
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}