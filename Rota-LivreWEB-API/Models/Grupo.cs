using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    public class Grupo
    {
        [Key]
        public int id_grupo { get; set; }

        public string link_grupo { get; set; } = string.Empty;
    }
}
