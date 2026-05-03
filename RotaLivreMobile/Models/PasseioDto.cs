public class PasseioDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public string Funcionamento { get; set; }
    public string ImagemUrl { get; set; }
    public int QuantidadeCurtidas { get; set; }
    public bool UsuarioJaCurtiu { get; set; }
    public bool UsuarioJaPendente { get; set; }

    public ImageSource ImagemSource
    {
        get
        {
            if (string.IsNullOrEmpty(ImagemUrl))
                return null;

            return ImageSource.FromUri(new Uri(ImagemUrl));
        }
    }
}