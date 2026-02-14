using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Rota_LivreWEB_API.Models
{
    public class Usuario
    {
        [Key]
        public int id_usuario { get; set; }

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

        [Required(ErrorMessage = "A resposta é obrigatória")]
        public string resposta_seg { get; set; }

        public int id_pergunta { get; set; }

        [ForeignKey(nameof(id_pergunta))]
        public PerguntaSeguranca? PerguntaSeguranca { get; set; }


        public Usuario()
        {
        }

        public Usuario(string Novo_Usuario_Nome, DateOnly Novo_Usuario_Nasc, string Novo_Usuario_Email, string Novo_Usuario_Senha, string Novo_Usuario_Resposta_Seg)
        {
            nome_completo = Novo_Usuario_Nome;
            data_nasc = Novo_Usuario_Nasc;
            email = Novo_Usuario_Email;
            senha = Novo_Usuario_Senha;
            resposta_seg = Novo_Usuario_Resposta_Seg;
     
        }
    }
}
