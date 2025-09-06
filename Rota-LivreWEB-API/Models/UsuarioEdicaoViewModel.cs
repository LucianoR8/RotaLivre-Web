using System;
using System.ComponentModel.DataAnnotations;


namespace Rota_LivreWEB_API.Models
{
    public class UsuarioEdicaoViewModel
{
    public int id_usuario { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string nome_completo { get; set; }

    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    public DateTime data_nasc { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
    public string email { get; set; }
}


}
