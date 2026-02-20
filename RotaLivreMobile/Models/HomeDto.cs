using RotaLivreMobile.Models;

public class HomeDto
{
    public string NomeUsuario { get; set; }
    public List<CategoriaDto> Categorias { get; set; }
    public List<PasseioDto> Destaques { get; set; }
}