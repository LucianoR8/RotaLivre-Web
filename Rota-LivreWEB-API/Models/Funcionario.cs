using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    public class Funcionario
    {
        [Key]
        public int id_funcionario { get; set; }

        public string nome_funcionario { get; set; } = string.Empty;
    }
}
