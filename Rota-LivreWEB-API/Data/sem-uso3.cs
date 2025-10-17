using MySql.Data.MySqlClient;

namespace Rota_LivreWEB_API.Data
{
    public class Conexao
    {

        private string connectionString = "Server=127.0.0.1;Database=rotalivre;Uid=appuser;Pwd=xforce;";

        public MySqlConnection Conectar()
        {
            return new MySqlConnection(connectionString);
        }

    }
}
