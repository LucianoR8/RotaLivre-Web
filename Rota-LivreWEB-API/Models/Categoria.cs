using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rota_LivreWEB_API.Models
{
    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_categoria { get; set; }

        public string tipo_categoria { get; set; }
        public string img { get; set; }


        public Categoria()
        {

        }
        public Categoria(string Novo_tipo_Categoria, string Novo_img)
        {
            tipo_categoria = Novo_tipo_Categoria;
            img = Novo_img;

        }
    }


}