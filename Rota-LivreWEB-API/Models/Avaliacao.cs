namespace Rota_LivreWEB_API.Models
{
    public class Avaliacao
    {
        public int id_avaliacao { get; set; }
        public int id_passeio { get; set; }
        public int id_usuario { get; set; }
        public string feedback { get; set; }
        public int nota { get; set; }
        public DateTime data_feedback { get; set; }


        public string nome_completo { get; set; }

       

    }

}


