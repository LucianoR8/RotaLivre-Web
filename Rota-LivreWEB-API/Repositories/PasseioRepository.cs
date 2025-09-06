using MySql.Data.MySqlClient;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.Repositories
{
    public class PasseioRepository
    {
        private readonly string _connectionString = "Server=rotalivre.c30u6uc8o0pe.us-east-2.rds.amazonaws.com;Port=3306;Database=rotalivre;Uid=admin;Pwd=$Rotalivre1;";

        public Passeio ObterPasseioPorId(int id)
        {
            Passeio passeio = null;

            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();

                var sql = @"
            SELECT 
                p.id_passeio, p.nome_passeio, p.descricao, p.funcionamento, p.img_url,
                e.id_endereco, e.nome_rua, e.numero_rua, e.complemento, e.bairro, e.cep, e.id_passeio AS endereco_id_passeio
            FROM passeio p
            LEFT JOIN endereco e ON p.id_passeio = e.id_passeio
            WHERE p.id_passeio = @id";

                var comando = new MySqlCommand(sql, conexao);
                comando.Parameters.AddWithValue("@id", id);

                using (var reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        passeio = new Passeio
                        {
                            id_passeio = reader.GetInt32("id_passeio"),
                            nome_passeio = reader.GetString("nome_passeio"),
                            descricao = reader.GetString("descricao"),
                            funcionamento = reader.GetString("funcionamento"),
                            img_url = reader.GetString("img_url"),
                            Endereco = reader.IsDBNull(reader.GetOrdinal("id_endereco")) ? null : new Endereco
                            {
                                id_endereco = reader.GetInt32("id_endereco"),
                                nome_rua = reader.GetString("nome_rua"),
                                numero_rua = reader.GetInt32("numero_rua"),
                                complemento = reader.IsDBNull(reader.GetOrdinal("complemento")) ? null : reader.GetString("complemento"),
                                bairro = reader.IsDBNull(reader.GetOrdinal("bairro")) ? null : reader.GetString("bairro"),
                                cep = reader.GetInt32("cep"),
                                id_passeio = reader.GetInt32("endereco_id_passeio")
                            }
                        };
                    }
                }
            }

            return passeio;
        }

        public int ObterTotalCurtidas(int idPasseio)
        {
            int total = 0;

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM curtida_passeio WHERE id_passeio = @id", conn);
                cmd.Parameters.AddWithValue("@id", idPasseio);

                total = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return total;
        }

        public bool PasseioExiste(int idPasseio)
        {
            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();
                var query = "SELECT COUNT(*) FROM passeio WHERE id_passeio = @idPasseio";
                using (var comando = new MySqlCommand(query, conexao))
                {
                    comando.Parameters.AddWithValue("@idPasseio", idPasseio);
                    var resultado = Convert.ToInt32(comando.ExecuteScalar());
                    return resultado > 0;
                }
            }
        }


        public bool UsuarioJaCurtiu(int idUsuario, int idPasseio)
        {
            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();
                var query = "SELECT COUNT(*) FROM curtida_passeio WHERE id_usuario = @idUsuario AND id_passeio = @idPasseio";
                using (var comando = new MySqlCommand(query, conexao))
                {
                    comando.Parameters.AddWithValue("@idUsuario", idUsuario);
                    comando.Parameters.AddWithValue("@idPasseio", idPasseio);
                    var resultado = Convert.ToInt32(comando.ExecuteScalar());
                    return resultado > 0;
                }
            }
        }

        public bool AlternarCurtida(int idUsuario, int idPasseio)
        {
            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();

                
                var existeCmd = new MySqlCommand("SELECT COUNT(*) FROM curtida_passeio WHERE id_usuario = @idUsuario AND id_passeio = @idPasseio", conexao);
                existeCmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                existeCmd.Parameters.AddWithValue("@idPasseio", idPasseio);
                var existe = Convert.ToInt32(existeCmd.ExecuteScalar());

                if (existe > 0)
                {
                   
                    var deleteCmd = new MySqlCommand("DELETE FROM curtida_passeio WHERE id_usuario = @idUsuario AND id_passeio = @idPasseio", conexao);
                    deleteCmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                    deleteCmd.Parameters.AddWithValue("@idPasseio", idPasseio);
                    deleteCmd.ExecuteNonQuery();
                    return false; 
                }
                else
                {
                    
                    var insertCmd = new MySqlCommand("INSERT INTO curtida_passeio (id_passeio, id_usuario) VALUES (@idPasseio, @idUsuario)", conexao);
                    insertCmd.Parameters.AddWithValue("@idPasseio", idPasseio);
                    insertCmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                    insertCmd.ExecuteNonQuery();
                    return true; 
                }
            }
        }



        public List<Passeio> BuscarPasseioPorNome(string termo)
        {
            var lista = new List<Passeio>();

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT id_passeio, nome_passeio, descricao, img_url 
                         FROM passeio
                         WHERE nome_passeio LIKE @termo";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@termo", "%" + termo + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var passeio = new Passeio
                            {
                                id_passeio = reader["id_passeio"] != DBNull.Value ? Convert.ToInt32(reader["id_passeio"]) : 0,
                                nome_passeio = reader["nome_passeio"] != DBNull.Value ? reader["nome_passeio"].ToString() : string.Empty,
                                descricao = reader["descricao"] != DBNull.Value ? reader["descricao"].ToString() : string.Empty,
                                img_url = reader["img_url"] != DBNull.Value ? reader["img_url"].ToString() : string.Empty,
                                
                            };


                            lista.Add(passeio);
                        }
                    }
                }

                
                foreach (var passeio in lista)
                {
                    passeio.QuantidadeCurtidas = ObterTotalCurtidas(passeio.id_passeio);
                }
            }

            return lista;
        }

    }







}

