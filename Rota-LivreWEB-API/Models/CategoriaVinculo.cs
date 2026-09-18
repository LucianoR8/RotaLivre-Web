using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    public class CategoriaVinculo
    {
        public int id_cidade { get; set; }
        [ForeignKey("id_cidade")]
        public Categoria Cidade { get; set; }

        public int id_tema { get; set; }
        [ForeignKey("id_tema")]
        public Categoria Tema { get; set; }
    }
}