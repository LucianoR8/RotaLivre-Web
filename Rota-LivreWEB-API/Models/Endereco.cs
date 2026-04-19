using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    public class Endereco
    {
        [Key]
        public int id_endereco { get; set; }

        public string nome_rua { get; set; }
        public string numero_rua { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        [Column("latitude")]
        public double? Latitude { get; set; }
        [Column("longitude")]
        public double? Longitude { get; set; }
        public int id_passeio { get; set; }
    }

}
