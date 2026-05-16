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

        [ForeignKey("Criador")]
        public int id_criador { get; set; }
        [ForeignKey(nameof(id_criador))]
        public Usuario? Criador { get; set; }

        public DateTime data_criacao { get; set; } = DateTime.UtcNow;

        public string status { get; set; } = "CRIADO"; 

        public DateTime? data_inicio { get; set; }
        public DateTime? data_fim { get; set; }
        public int id_passeio { get; set; }
        [ForeignKey(nameof(id_passeio))]
        public Passeio? Passeio { get; set; }
        public ICollection<GrupoUsuario>? Usuarios { get; set; }
    }
}
