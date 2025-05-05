using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Digests;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.DbContext
{
    public class UsuarioDbContext
    {
        private const string StringDeConexao = "Server=rotalivre.c30u6uc8o0pe.us-east-2.rds.amazonaws.com;Port=3306;Database=rotalivre;Uid=admin;Pwd=$Rotalivre1;";

        public static string Cadastra_Usuario(Usuario NovoUsuario)
        {
            try
            {
                MySqlConnection conexao = new();

                conexao.ConnectionString = StringDeConexao;
                string query = "INSERT INTO usuario (nome_completo, data_nasc, email, senha) VALUES (@Novo_Usuario_Nome, @Novo_Usuario_Nasc, @Novo_Usuario_Email, @Novo_Usuario_Senha);";
                MySqlCommand comando = new(query, conexao);

                comando.Parameters.AddWithValue("@Novo_Usuario_Nome", NovoUsuario.nome_completo);
                comando.Parameters.AddWithValue("@Novo_Usuario_Nasc", NovoUsuario.data_nasc);
                comando.Parameters.AddWithValue("@Novo_Usuario_Email", NovoUsuario.email);
                comando.Parameters.AddWithValue("@Novo_Usuario_Senha", NovoUsuario.senha);
                conexao.Open();
                comando.ExecuteNonQuery();
                conexao.Close();
                return "Sucesso";
            }
            catch (Exception)
            {
                return "Falha";

            }



        }

        public static bool EmailExiste(string email)
        {
            try
            {
                using (var conexao = new MySqlConnection(StringDeConexao))
                {
                    string query = "SELECT COUNT(*) FROM usuario WHERE email = @Email";
                    MySqlCommand comando = new(query, conexao);
                    comando.Parameters.AddWithValue("@Email", email);

                    conexao.Open();
                    int resultado = Convert.ToInt32(comando.ExecuteScalar());
                    conexao.Close();

                    return resultado > 0; 
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool VerificarLogin(string email, string senha)
        {
            try
            {
                using (var conexao = new MySqlConnection(StringDeConexao))
                {
                    string query = "SELECT COUNT(*) FROM usuario WHERE email = @Email AND senha = @Senha";
                    MySqlCommand comando = new(query, conexao);
                    comando.Parameters.AddWithValue("@Email", email);
                    comando.Parameters.AddWithValue("@Senha", senha);

                    conexao.Open();
                    int resultado = Convert.ToInt32(comando.ExecuteScalar());
                    conexao.Close();

                    return resultado > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro no login: " + ex.Message);
                return false;
            }
        }

        public static int BuscarIdPorEmail(string email)
        {
            try
            {
                using (var conexao = new MySqlConnection(StringDeConexao))
                {
                    string query = "SELECT id_usuario FROM usuario WHERE email = @Email LIMIT 1";
                    MySqlCommand comando = new(query, conexao);
                    comando.Parameters.AddWithValue("@Email", email);

                    conexao.Open();
                    var resultado = comando.ExecuteScalar();
                    conexao.Close();

                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        public static Usuario BuscarUsuarioPorId(int id)
        {
            try
            {
                using (var conexao = new MySqlConnection(StringDeConexao))
                {
                    string query = "SELECT * FROM usuario WHERE id_usuario = @Id LIMIT 1";
                    MySqlCommand comando = new(query, conexao);
                    comando.Parameters.AddWithValue("@Id", id);

                    conexao.Open();
                    MySqlDataReader reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        Usuario u = new Usuario
                        {
                            id_usuario = reader.GetInt32("id_usuario"),
                            nome_completo = reader.GetString("nome_completo"),
                            data_nasc = reader.GetDateTime("data_nasc"),
                            email = reader.GetString("email"),
                            senha = reader.GetString("senha")
                        };

                        conexao.Close();
                        return u;
                    }

                    conexao.Close();
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static void AtualizarUsuario(Usuario usuario)
        {
            using (var conexao = new MySqlConnection(StringDeConexao))
            {
                string query = @"UPDATE usuario SET nome_completo = @Nome, data_nasc = @Nasc, email = @Email, senha = @Senha
                         WHERE id_usuario = @Id";

                MySqlCommand comando = new(query, conexao);
                comando.Parameters.AddWithValue("@Nome", usuario.nome_completo);
                comando.Parameters.AddWithValue("@Nasc", usuario.data_nasc);
                comando.Parameters.AddWithValue("@Email", usuario.email);
                comando.Parameters.AddWithValue("@Senha", usuario.senha);
                comando.Parameters.AddWithValue("@Id", usuario.id_usuario);

                conexao.Open();
                Console.WriteLine($"Atualizando usuário ID {usuario.id_usuario} - Nome: {usuario.nome_completo}");
                comando.ExecuteNonQuery();
                conexao.Close();
            }
        }


        public static void DeletarUsuario(int id)
        {
            using (var conexao = new MySqlConnection(StringDeConexao))
            {
                string query = "DELETE FROM usuario WHERE id_usuario = @Id";
                MySqlCommand comando = new(query, conexao);
                comando.Parameters.AddWithValue("@Id", id);

                conexao.Open();
                comando.ExecuteNonQuery();
                conexao.Close();
            }
        }

    }
}