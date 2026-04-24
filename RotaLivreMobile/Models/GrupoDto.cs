using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotaLivreMobile.Models
{
    public class GrupoDto
    {
        public int PasseioId { get; set; }
        public string NomePasseio { get; set; }
        public List<string> Usuarios { get; set; }
    }
}
