using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    public class GrupoPasseio
    {

        [Key]
        public int id_grupo_passeio { get; set; }

        [ForeignKey("Grupo")]
        public int id_grupo { get; set; }

        [ForeignKey("Passeio")]
        public int id_passeio { get; set; }

        public Grupo? Grupo { get; set; }
        public Passeio? Passeio { get; set; }

    }
}
