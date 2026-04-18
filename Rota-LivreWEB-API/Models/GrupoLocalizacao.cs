using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    [Table("grupolocalizacao")]
    public class GrupoLocalizacao
    {
        [Key]
        public int id_localizacao { get; set; }

        [ForeignKey("Grupo")]
        public int id_grupo { get; set; }

        [ForeignKey("Usuario")]
        public int id_usuario { get; set; }

        public double latitude { get; set; }
        public double longitude { get; set; }

        public DateTime data_envio { get; set; } = DateTime.UtcNow;

        public Grupo? Grupo { get; set; }
        public Usuario? Usuario { get; set; }
    }
}