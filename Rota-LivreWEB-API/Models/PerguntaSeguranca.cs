using System.ComponentModel.DataAnnotations;

namespace Rota_LivreWEB_API.Models
{
    public class PerguntaSeguranca
    {
        [Key]
        public int id_pergunta { get; set; }
        public string pergunta_seg { get; set; }
    }
}
