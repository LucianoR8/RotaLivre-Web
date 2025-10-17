using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Digests;
using Rota_LivreWEB_API.Models;
using System.Security.Cryptography;
using System.Text;


namespace Rota_LivreWEB_API.Data
{
    public class UsuarioDbContext
    {
        private readonly Conexao _conexao;
        public UsuarioDbContext(Conexao conexao)
        {
            _conexao = conexao;
        }
        public string Cadastra_Usuario(Usuario NovoUsuario)
        {
            try
            {
                using (var conexao = _conexao.Conectar())
                {
                    string query = "INSERT INTO usuario (nome_completo, data_nasc, email, senha, resposta_seg, id_pergunta) VALUES (@Novo_Usuario_Nome, @Novo_Usuario_Nasc, @Novo_Usuario_Email, @Novo_Usuario_Senha, @Novo_Usuario_Resposta_Seg, @id_pergunta);";
                    MySqlCommand comando = new(query, conexao);

                    comando.Parameters.AddWithValue("@Novo_Usuario_Nome", NovoUsuario.nome_completo);
                    comando.Parameters.AddWithValue("@Novo_Usuario_Nasc", NovoUsuario.data_nasc);
                    comando.Parameters.AddWithValue("@Novo_Usuario_Email", NovoUsuario.email);
                    string senhaHash = GerarHash(NovoUsuario.senha);
                    comando.Parameters.AddWithValue("@Novo_Usuario_Senha", senhaHash);
                    string respostaHash = GerarHash(NovoUsuario.resposta_seg);
                    comando.Parameters.AddWithValue("@Novo_Usuario_Resposta_Seg", respostaHash);
                    comando.Parameters.AddWithValue("@id_pergunta", NovoUsuario.id_pergunta);
                    conexao.Open();
                    comando.ExecuteNonQuery();
                    conexao.Close();
                    return "Sucesso";
                }
            }
            catch (Exception)
            {
                return "Falha";

            }
        }

        public bool EmailExiste(string email)
        {
            try
            {
                using (var conexao = _conexao.Conectar())
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

        public bool VerificarLogin(string email, string senha)
        {
            try
            {
                using (var conexao = _conexao.Conectar())
                {
                    string query = "SELECT COUNT(*) FROM usuario WHERE email = @Email AND senha = @Senha";
                    MySqlCommand comando = new(query, conexao);
                    comando.Parameters.AddWithValue("@Email", email);
                    string senhaHash = GerarHash(senha);
                    comando.Parameters.AddWithValue("@Senha", senhaHash);


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

        public static string GerarHash(string senha)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(senha);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToHexString(hashBytes); 
            }
        }


        public int BuscarIdPorEmail(string email)
        {
            try
            {
                using (var conexao = _conexao.Conectar())
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

        public Usuario BuscarUsuarioPorId(int id)
        {
            try
            {
                using (var conexao = _conexao.Conectar())
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
                            senha = reader.GetString("senha"),
                            resposta_seg = reader.GetString("resposta_seg")
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

        public void AtualizarUsuario(Usuario usuario)
        {
            using (var conexao = _conexao.Conectar())
            {
                conexao.Open();

                string query = @"UPDATE usuario 
                         SET nome_completo = @nome, email = @mail, data_nasc = @data 
                         WHERE id_usuario = @id";

                using (var cmd = new MySqlCommand(query, conexao))
                {
                    cmd.Parameters.AddWithValue("@nome", usuario.nome_completo);
                    cmd.Parameters.AddWithValue("@mail", usuario.email);
                    cmd.Parameters.AddWithValue("@data", usuario.data_nasc);
                    cmd.Parameters.AddWithValue("@id", usuario.id_usuario);

                    cmd.ExecuteNonQuery();
                }
            }
        }



        public void DeletarUsuario(int id)
        {
            using (var conexao = _conexao.Conectar())
            {
                string query = "DELETE FROM usuario WHERE id_usuario = @Id";
                MySqlCommand comando = new(query, conexao);
                comando.Parameters.AddWithValue("@Id", id);

                conexao.Open();
                comando.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public string BuscarNomePorEmail(string email)
        {
            using (var conexao = _conexao.Conectar())
            {
                conexao.Open();
                var query = "SELECT nome_completo FROM usuario WHERE email = @Email";
                using (var command = new MySqlCommand(query, conexao))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    var resultado = command.ExecuteScalar();
                    return resultado?.ToString() ?? "";
                }
            }
        }

        public Usuario BuscarUsuarioPorEmail(string email)
        {
            try
            {
                using (var conexao = _conexao.Conectar())
                {
                    string query = "SELECT * FROM usuario WHERE email = @Email LIMIT 1";
                    MySqlCommand comando = new(query, conexao);
                    comando.Parameters.AddWithValue("@Email", email);

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
                            senha = reader.GetString("senha"),
                            resposta_seg = reader.GetString("resposta_seg")
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

        public bool AlterarSenha(int idUsuario, string novaSenha)
        {
            using (var conexao = _conexao.Conectar())
            {
                conexao.Open();
                var query = "UPDATE usuario SET senha = @novaSenha WHERE id_usuario = @id";
                using (var cmd = new MySqlCommand(query, conexao))
                {
                    string senhaHash = GerarHash(novaSenha);
                    cmd.Parameters.AddWithValue("@novaSenha", senhaHash);
                    cmd.Parameters.AddWithValue("@id", idUsuario);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<PerguntaSeguranca> ObterPerguntasDeSeguranca()
        {
            List<PerguntaSeguranca> perguntas = new();

            using (var conexao = _conexao.Conectar())
            {

                string query = "SELECT id_pergunta, pergunta_seg FROM perguntaseguranca";
                MySqlCommand comando = new(query, conexao);
                conexao.Open();
                MySqlDataReader reader = comando.ExecuteReader();



                while (reader.Read())
                {
                    perguntas.Add(new PerguntaSeguranca
                    {
                        id_pergunta = reader.GetInt32("id_pergunta"),
                        pergunta_seg = reader.GetString("pergunta_seg")
                    });
                }


                conexao.Close();
                return perguntas;

            }
        }

        public string BuscarPerguntaDoUsuario(string email)
        {
            using (var conexao = _conexao.Conectar())
            {

                string query = @"SELECT p.pergunta_seg 
                     FROM usuario u 
                     JOIN perguntaseguranca p ON u.id_pergunta = p.id_pergunta 
                     WHERE u.email = @Email";

                MySqlCommand comando = new(query, conexao);
                comando.Parameters.AddWithValue("@Email", email);

                conexao.Open();
                object resultado = comando.ExecuteScalar();
                conexao.Close();

                return resultado?.ToString();
            }
        }

        public string ObterRespostaDoBanco(string email)
        {
            using (var conexao = _conexao.Conectar())
            {

                string query = "SELECT resposta_seg FROM usuario WHERE email = @Email";
                MySqlCommand comando = new(query, conexao);
                comando.Parameters.AddWithValue("@Email", email);

                conexao.Open();
                object resultado = comando.ExecuteScalar();
                conexao.Close();

                return resultado?.ToString();
            }
        }






    }
}