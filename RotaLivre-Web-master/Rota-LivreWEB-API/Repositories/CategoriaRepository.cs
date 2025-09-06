using MySql.Data.MySqlClient;
using Rota_LivreWEB_API.Models;
using System.Collections.Generic;

namespace Rota_LivreWEB_API.Repositories
{
    public class CategoriaRepository
    {
        private readonly string _connectionString = "Server=serverless-northeurope.sysp0000.db3.skysql.com;Port=4014;Database=rota_livre;Uid=dbpbf31374672;Pwd=vEPPCZMY85?UdNIc9bLsho73;";

        public List<Categoria> ObterCategorias()
        {
            var lista = new List<Categoria>();

            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();
                var comando = new MySqlCommand("SELECT id_categoria, tipo_categoria, img FROM categoria", conexao);
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
    }
}
