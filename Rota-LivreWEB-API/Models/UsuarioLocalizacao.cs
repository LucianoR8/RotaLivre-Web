using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    public class UsuarioLocalizacao
    {
        [Key]
        public int id_usuario_localizacao { get; set; }

        [ForeignKey("Usuario")]
        public int id_usuario { get; set; }

        [ForeignKey("Localizacao")]
        public int id_localizacao { get; set; }

        public Usuario? Usuario { get; set; }
        public Localizacao? Localizacao { get; set; }
    }
}
