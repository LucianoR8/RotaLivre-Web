using System.ComponentModel.DataAnnotations;

namespace Rota_LivreWEB_API.Models
{
    public class UsuarioCreateViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string nome_completo { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateOnly data_nasc { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string senha { get; set; }

        [Required(ErrorMessage = "A confirmação de senha é obrigatória.")]
        [Compare("senha", ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmarSenha { get; set; }

        [Required(ErrorMessage = "A resposta é obrigatória")]
        public string resposta_seg { get; set; }

        public int id_pergunta { get; set; }
    }
}
