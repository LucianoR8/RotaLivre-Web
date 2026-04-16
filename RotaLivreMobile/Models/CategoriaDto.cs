using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RotaLivreMobile.Helpers;

namespace RotaLivreMobile.Models
{
    public class CategoriaDto
    {
        public int IdCategoria { get; set; }
        public string TipoCategoria { get; set; }
        public string ImgUrl { get; set; }
        public string ImagemCompleta => $"{AppConfig.ServerUrl}{ImgUrl}";
    }
}
