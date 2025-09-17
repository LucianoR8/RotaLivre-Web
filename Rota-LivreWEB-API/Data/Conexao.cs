using MySql.Data.MySqlClient;

namespace Rota_LivreWEB_API.Data
{
    public class Conexao
    {

        private string connectionString = "Server=trocar;Database=trocar;Uid=appuser;Pwd=trocar;";

        public MySqlConnection Conectar()
        {
            return new MySqlConnection(connectionString);
        }

    }
}
