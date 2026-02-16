using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RotaLivreMobile.Services;

namespace RotaLivreMobile.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        public HomeViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task TestarApi()
        {
            var response = await _apiService.GetAsync("AuthApi/teste");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
            }
            else
            {
                Console.WriteLine("Erro: " + response.StatusCode);
            }
        }
    }
}
