using MySql.Data.MySqlClient;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.Models;
using System.Collections.Generic;

namespace Rota_LivreWEB_API.Repositories
{
    public class CategoriaRepository
    {
        private readonly Conexao _conexao;

        public CategoriaRepository(Conexao conexao)
        {
            _conexao = conexao;
        }
        public List<Categoria> ObterCategorias()
        {
            var lista = new List<Categoria>();

            using (var conexao =  _conexao.Conectar())
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
