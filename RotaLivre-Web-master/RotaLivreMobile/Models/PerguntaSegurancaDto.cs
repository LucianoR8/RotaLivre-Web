using System.Text.Json.Serialization;

public class PerguntaSegurancaDto
{
    [JsonPropertyName("id_pergunta")]
    public int Id_Pergunta { get; set; }

    [JsonPropertyName("pergunta_seg")]
    public string pergunta_seg { get; set; }

    public override string ToString()
    {
        return pergunta_seg;
    }
}