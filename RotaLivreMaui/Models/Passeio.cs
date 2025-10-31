namespace RotaLivreMaui.Models
{
    public class Passeio
    {
        public int id_passeio { get; set; }
        public string nome_passeio { get; set; }
        public string descricao { get; set; }
        public string horario_funcionamento { get; set; }
        public string img_url { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
    }
}
