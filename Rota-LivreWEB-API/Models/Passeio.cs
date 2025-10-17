using System.ComponentModel.DataAnnotations;

namespace Rota_LivreWEB_API.Models
{
    public class Passeio
    {
        [Key]
        public int id_passeio { get; set; }

        public int id_categoria { get; set; }
        public string nome_passeio { get; set; }
        public string funcionamento { get; set; }
        public string descricao { get; set; }
        public string img_url { get; set; }
        public int? Enderecoid_endereco { get; set; }  
        public Endereco? Endereco { get; set; }         


        public int QuantidadeCurtidas { get; set; }

        
        public bool AlternarCurtida { get; set; }
        public bool UsuarioJaCurtiu { get; set; }
        public bool UsuarioJaPendente { get; set; }



        public Passeio()
        {
        }

        public Passeio(int? id_categoria, string nome_passeio, string funcionamento, string descricao, string img_url)
        {
            this.id_categoria = id_categoria ?? 0;
            this.nome_passeio = nome_passeio;
            this.funcionamento = funcionamento;
            this.descricao = descricao;
            this.img_url = img_url;
        }

    }
}
