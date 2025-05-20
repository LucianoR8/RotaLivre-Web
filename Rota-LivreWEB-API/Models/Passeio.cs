namespace Rota_LivreWEB_API.Models
{
    public class Passeio
    {
        public int id_passeio { get; set; }
        public int id_categoria { get; set; }
        public string nome_passeio { get; set; }
        public bool ida_e_volta { get; set; }
        public DateTime data_hora_ida { get; set; }
        public DateTime? data_hora_volta { get; set; }
        public string funcionamento { get; set; }
        public string descricao { get; set; }
        public string img_url { get; set; }

        public Endereco Endereco { get; set; }

        public int QuantidadeCurtidas { get; set; }

        
        public bool AlternarCurtida { get; set; }
        public bool UsuarioJaCurtiu { get; set; }



        public Passeio()
        {
        }

        public Passeio(int? id_categoria, string nome_passeio, bool? ida_e_volta, DateTime? data_hora_ida, DateTime? data_hora_volta, string funcionamento, string descricao, string img_url)
        {
            this.id_categoria = id_categoria ?? 0;
            this.nome_passeio = nome_passeio;
            this.ida_e_volta = ida_e_volta ?? false;
            this.data_hora_ida = data_hora_ida ?? DateTime.Now;
            this.data_hora_volta = data_hora_volta;
            this.funcionamento = funcionamento;
            this.descricao = descricao;
            this.img_url = img_url;
        }

    }
}
