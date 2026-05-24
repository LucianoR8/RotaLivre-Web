using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    public class Localizacao
    {
        [Key]
        public int id_localizacao { get; set; }

        public double latitude { get; set; }
        public double longitude { get; set; }
        public double? velocidade { get; set; }
        public DateTime data_registro { get; set; }
    }
}
