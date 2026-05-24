using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    public class PasseioFavorito
    {
        [Key]
        public int id_favorito { get; set; }

        [ForeignKey("Usuario")]
        public int id_usuario { get; set; }

        [ForeignKey("Passeio")]
        public int id_passeio { get; set; }

        public Usuario? Usuario { get; set; }
        public Passeio? Passeio { get; set; }

    }
}
