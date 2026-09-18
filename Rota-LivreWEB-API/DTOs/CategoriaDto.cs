using System.Collections.Generic;

namespace Rota_LivreWEB_API.DTOs
{
    public class CategoriaDto
    {
        public int IdCategoria { get; set; }
        public string TipoCategoria { get; set; }
        public string ImgUrl { get; set; }
        public bool Ativo { get; set; }

        // NOVO: Define se é 'CIDADE' ou 'TEMA'
        public string Classificacao { get; set; } = "TEMA";

        // NOVO: Array com os IDs das cidades marcadas no painel
        public List<int> CidadesVinculadas { get; set; } = new List<int>();
    }
}