namespace Rota_LivreWEB_API.DTOs
{
    public class EnderecoDto
    {
        public string NomeRua { get; set; }
        public string NumeroRua { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int RaioMetros { get; set; }

    }

}
