using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Rota_LivreWEB_API.Models
{
    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_categoria { get; set; }

        public string tipo_categoria { get; set; }
        public string img { get; set; }

        public bool ativo { get; set; } = true;
        public int? atualizado_por { get; set; }
        public DateTime? atualizado_em { get; set; }

        [ForeignKey("atualizado_por")]
        public Usuario? AdminAtualizacao { get; set; }

        public string classificacao { get; set; } = "TEMA";

        // Listas para o EF Core navegar no relacionamento N:M
        public ICollection<CategoriaVinculo> VinculosComoCidade { get; set; } = new List<CategoriaVinculo>();
        public ICollection<CategoriaVinculo> VinculosComoTema { get; set; } = new List<CategoriaVinculo>();

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