using System.Data;
using MySql.Data.MySqlClient;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.DbContext
{
    public class PasseioDb
    {
        private const string StringDeConexao = "Server=rotalivre.c30u6uc8o0pe.us-east-2.rds.amazonaws.com;Port=3306;Database=rotalivre;Uid=admin;Pwd=$Rotalivre1;";

        public static List<Passeio> BuscarPasseiosPorCategoria(int idCategoria)
        {
            var lista = new List<Passeio>();

            using (var conexao = new MySqlConnection(StringDeConexao))
            {
                string query = "SELECT * FROM passeio WHERE id_categoria = @id";
                MySqlCommand comando = new(query, conexao);
                comando.Parameters.AddWithValue("@id", idCategoria);

                conexao.Open();
                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Passeio
                        {
                            id_passeio = reader["id_passeio"] != DBNull.Value ? Convert.ToInt32(reader["id_passeio"]) : 0,
                            nome_passeio = reader["nome_passeio"] != DBNull.Value ? reader["nome_passeio"].ToString() : "",
                            descricao = reader["descricao"] != DBNull.Value ? reader["descricao"].ToString() : "",
                            funcionamento = reader["funcionamento"] != DBNull.Value ? reader["funcionamento"].ToString() : "",
                            img_url = reader["img_url"] != DBNull.Value ? reader["img_url"].ToString() : ""
                        });

                    }
                }
            }

            return lista;
        }

        public static Passeio BuscarPasseioPorId(int id)
        {
            using (var conexao = new MySqlConnection(StringDeConexao))
            {
                string query = "SELECT * FROM passeio WHERE id_passeio = @id";
                MySqlCommand comando = new(query, conexao);
                comando.Parameters.AddWithValue("@id", id);

                conexao.Open();
                using (var reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Passeio
                        {
                            id_passeio = reader.GetInt32("id_passeio"),
                            id_categoria = reader.GetInt32("id_categoria"),
                            nome_passeio = reader.GetString("nome_passeio"),
                            ida_e_volta = reader.GetBoolean("ida_e_volta"),
                            data_hora_ida = reader.GetDateTime("data_hora_ida"),
                            data_hora_volta = reader.IsDBNull("data_hora_volta") ? null : reader.GetDateTime("data_hora_volta"),
                            funcionamento = reader.GetString("funcionamento"),
                            descricao = reader.GetString("descricao"),
                            img_url = reader.GetString("img_url")
                        };
                    }
                }
            }

            return null;
        }

        public static List<Categoria> BuscarCategorias()
        {
            var lista = new List<Categoria>();

            using (var conexao = new MySqlConnection(StringDeConexao))
            {
                string query = "SELECT * FROM categoria";
                MySqlCommand comando = new(query, conexao);

                conexao.Open();
                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Categoria
                        {
                            id_categoria = reader.GetInt32("id_categoria"),
                            tipo_categoria = reader.GetString("tipo_categoria"),
                            img = reader.GetString("img")
                        });
                    }
                }
            }

            return lista;
        }

        public static void InserirAvaliacao(int idPasseio, int idUsuario, int nota, string feedback)
        {
            string conexao = "Server=rotalivre.c30u6uc8o0pe.us-east-2.rds.amazonaws.com;Port=3306;Database=rotalivre;Uid=admin;Pwd=$Rotalivre1;";

            using (MySqlConnection conn = new MySqlConnection(conexao))
            {
                conn.Open();
                string sql = @"INSERT INTO avaliacao (id_passeio, id_usuario, nota, feedback, data_feedback)
                       VALUES (@id_passeio, @id_usuario, @nota, @feedback, NOW())";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id_passeio", idPasseio);
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Parameters.AddWithValue("@nota", nota);
                    cmd.Parameters.AddWithValue("@feedback", feedback);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<Avaliacao> ListarAvaliacoesPorPasseio(int idPasseio)
        {
            var lista = new List<Avaliacao>();

            using (var conexao = new MySqlConnection(StringDeConexao))
            {
                conexao.Open();
                string sql = @"SELECT a.id_avaliacao, a.id_passeio, a.id_usuario, u.nome_completo, a.feedback, a.nota, a.data_feedback
                       FROM avaliacao a
                       JOIN usuario u ON a.id_usuario = u.id_usuario
                       WHERE a.id_passeio = @idPasseio
                       ORDER BY a.data_feedback DESC";

                using (var cmd = new MySqlCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@idPasseio", idPasseio);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var avaliacao = new Avaliacao
                            {
                                id_avaliacao = Convert.ToInt32(reader["id_avaliacao"]),
                                id_passeio = Convert.ToInt32(reader["id_passeio"]),
                                id_usuario = Convert.ToInt32(reader["id_usuario"]),
                                nome_completo = reader["nome_completo"].ToString(),
                                feedback = reader["feedback"].ToString(),
                                nota = Convert.ToInt32(reader["nota"]),
                                data_feedback = Convert.ToDateTime(reader["data_feedback"])
                            };

                            lista.Add(avaliacao);
                        }
                    }
                }
            }

            return lista;
        }



    }
}
