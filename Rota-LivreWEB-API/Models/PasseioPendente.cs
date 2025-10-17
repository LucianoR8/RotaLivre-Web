using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    [Table("passeiospendentes")]
    public class PasseioPendente
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("Usuario")]
        public int id_usuario { get; set; }

        [ForeignKey("Passeio")]
        public int id_passeio { get; set; }

        public DateTime data_adicao { get; set; } = DateTime.Now;

        public Usuario Usuario { get; set; }
        public Passeio Passeio { get; set; }

    }
}
