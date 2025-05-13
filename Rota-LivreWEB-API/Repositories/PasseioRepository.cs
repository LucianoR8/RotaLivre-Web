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


    }
}
