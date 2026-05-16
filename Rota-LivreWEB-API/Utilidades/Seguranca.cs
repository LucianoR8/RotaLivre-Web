using System.Security.Cryptography;
using System.Text;

namespace Rota_LivreWEB_API.Utilidades.Seguranca
{
    public static class HashHelper
    {
        public static string GerarHash(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(texto);
            var hash = sha256.ComputeHash(bytes);

            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}
