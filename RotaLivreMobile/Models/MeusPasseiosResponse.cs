using System;
using System.Collections.Generic;
using System.Text;

namespace RotaLivreMobile.Models
{
    public class MeusPasseiosResponse
    {
        public List<PasseioDto> Curtidos { get; set; } = new();
        public List<PasseioDto> Pendentes { get; set; } = new();
    
    }
}
