namespace Rota_LivreWEB_API.DTOs
{
    public class LocalizacaoDto
    {
        public int UsuarioId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}