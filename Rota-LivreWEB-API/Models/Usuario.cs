using System;
using System.ComponentModel.DataAnnotations;

namespace Rota_LivreWEB_API.Models
{
    public class Usuario
    {
        public int id_usuario { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string nome_completo { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime data_nasc { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string senha { get; set; }

        [Required(ErrorMessage = "A confirmação de senha é obrigatória.")]
        [DataType(DataType.Password)]
        [Compare("senha", ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmarSenha { get; set; }

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
