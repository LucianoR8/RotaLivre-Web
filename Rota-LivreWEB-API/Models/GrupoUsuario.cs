using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    public class GrupoUsuario
    {
        [Key]
        public int id_grupo_usuario { get; set; }

        [ForeignKey("Grupo")]
        public int id_grupo { get; set; }

        [ForeignKey("Usuario")]
        public int id_usuario { get; set; }

        public Grupo? Grupo { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
