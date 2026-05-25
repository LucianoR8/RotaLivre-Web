namespace Rota_LivreWEB_API.DTOs
{
    public class UsuarioCadastroDto
    {
        public string Nome { get; set; }
        public string DataNasc { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public int IdPergunta { get; set; }
        public string RespostaSeg { get; set; }
    }
}