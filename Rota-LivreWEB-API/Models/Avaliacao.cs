using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    public class Avaliacao
    {
        [Key]
        public int id_avaliacao { get; set; }

        [ForeignKey("Passeio")]
        public int id_passeio { get; set; }

        [ForeignKey("Usuario")]
        public int id_usuario { get; set; }

        public string feedback { get; set; }
        public int nota { get; set; }
        public DateTime data_feedback { get; set; }


        public string nome_completo { get; set; }

        public Usuario Usuario { get; set; }
        public Passeio Passeio { get; set; }



    }

}


