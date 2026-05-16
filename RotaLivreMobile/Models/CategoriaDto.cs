using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotaLivreMobile.Models
{
    public class CategoriaDto
    {
        public int IdCategoria { get; set; }
        public string TipoCategoria { get; set; }
        public string ImgUrl { get; set; }

        public ImageSource ImgSource
        {
            get
            {
                if (string.IsNullOrEmpty(ImgUrl))
                    return null;

                return ImageSource.FromUri(new Uri(ImgUrl));
            }
        }
    }
}
