using MySql.Data.MySqlClient;
using Rota_LivreWEB_API.Models;
using System.Collections.Generic;

namespace Rota_LivreWEB_API.Repositories
{
    public class CategoriaRepository
    {
        private readonly string _connectionString = "Server=rotalivre.c30u6uc8o0pe.us-east-2.rds.amazonaws.com;Port=3306;Database=rotalivre;Uid=admin;Pwd=$Rotalivre1;";

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
