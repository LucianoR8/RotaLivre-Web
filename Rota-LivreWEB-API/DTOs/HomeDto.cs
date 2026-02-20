using System.Collections.Generic;

namespace Rota_LivreWEB_API.DTOs
{
    public class HomeDto
    {
        public string NomeUsuario { get; set; }
        public List<CategoriaDto> Categorias { get; set; }
        public IEnumerable<PasseioDto> Destaques { get; set; }

    }
}