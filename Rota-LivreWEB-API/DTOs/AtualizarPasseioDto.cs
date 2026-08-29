namespace Rota_LivreWEB_API.DTOs
{
    public class AtualizarPasseioDto
    {
        public string Nome { get; set; }
        public int CategoriaId { get; set; }
        public string Descricao { get; set; }
        public string Funcionamento { get; set; }
        public string ImagemUrl { get; set; }
        public string Status { get; set; }
    }
}