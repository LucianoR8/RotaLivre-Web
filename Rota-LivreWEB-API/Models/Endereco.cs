namespace Rota_LivreWEB_API.Models
{
    public class Endereco
    {
        public int id_endereco { get; set; }
        public string nome_rua { get; set; }
        public int numero_rua { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public int cep { get; set; }
        public int id_passeio { get; set; }
    }

}
