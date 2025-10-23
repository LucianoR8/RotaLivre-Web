using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotaLivreMaui.Models;

public class PasseioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Raio { get; set; } 
}
