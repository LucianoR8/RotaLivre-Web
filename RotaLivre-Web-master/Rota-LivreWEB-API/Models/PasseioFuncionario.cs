using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    public class PasseioFuncionario
    {
        [Key]
        public int id_passeio_funcionario { get; set; }

        [ForeignKey("Passeio")]
        public int id_passeio { get; set; }

        [ForeignKey("Funcionario")]
        public int id_funcionario { get; set; }

        public Passeio? Passeio { get; set; }
        public Funcionario? Funcionario { get; set; }
    }
}
