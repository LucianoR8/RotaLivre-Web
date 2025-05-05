namespace Rota_LivreWEB_API.Models
{
    public class Usuario
    {
        public int id_usuario { get; set; }
        public string nome_completo { get; set; }
        public DateTime data_nasc { get; set; }
        public string email { get; set; }
        public string senha { get; set; }


        public Usuario()
        {
        }

        public Usuario(string Novo_Usuario_Nome, DateTime Novo_Usuario_Nasc, string Novo_Usuario_Email, string Novo_Usuario_Senha)
        {
            nome_completo = Novo_Usuario_Nome;
            data_nasc = Novo_Usuario_Nasc;
            email = Novo_Usuario_Email;
            senha = Novo_Usuario_Senha;
        }
    }
}