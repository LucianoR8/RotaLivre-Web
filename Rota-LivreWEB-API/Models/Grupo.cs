using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    public class Grupo
    {
        [Key]
        public int id_grupo { get; set; }

        public string nome { get; set; } = string.Empty;

        public string codigo_convite { get; set; } = string.Empty;

        public int id_criador { get; set; }

        public DateTime data_criacao { get; set; } = DateTime.UtcNow;

        public bool ativo { get; set; } = true;

        public bool em_andamento { get; set; }

        public DateTime? data_inicio { get; set; }
        public DateTime? data_fim { get; set; }

        public ICollection<GrupoUsuario>? Usuarios { get; set; }
    }
}
