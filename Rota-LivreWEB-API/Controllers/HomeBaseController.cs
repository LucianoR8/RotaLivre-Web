using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Rota_LivreWEB_API.Repositories;
using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.DbContext;

namespace Rota_LivreWEB_API.Controllers
{
    public class HomeBaseController : Controller
    {
        private readonly CategoriaRepository _repo = new CategoriaRepository();
        private readonly string connectionString = "Server=rotalivre.c30u6uc8o0pe.us-east-2.rds.amazonaws.com;Port=3306;Database=rotalivre;Uid=admin;Pwd=$Rotalivre1;";

        public ViewResult Home()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario != null)
            {
                var usuario = BuscarUsuarioPorId(idUsuario.Value);
                ViewBag.NomeUsuario = usuario?.nome_completo ?? "Usuário";
            }
            else
            {
                ViewBag.NomeUsuario = "Visitante";
            }

            var categorias = _repo.ObterCategorias();
            var passeiosEmDestaque = BuscarPasseiosMaisCurtidos();

            ViewBag.PasseiosDestaque = passeiosEmDestaque;

            return View(categorias);
        }

        public ActionResult PasseiosPorCategoria(int id)
        {
            var passeios = PasseioDb.BuscarPasseiosPorCategoria(id);
            ViewBag.CategoriaId = id;
            return View(passeios);
        }

        private Usuario BuscarUsuarioPorId(int id)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT id_usuario, nome_completo FROM usuario WHERE id_usuario = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Usuario
                        {
                            id_usuario = reader.GetInt32("id_usuario"),
                            nome_completo = reader.GetString("nome_completo")
                        };
                    }
                }
            }
            return null;
        }

        private List<Passeio> BuscarPasseiosMaisCurtidos()
        {
            var lista = new List<Passeio>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                var cmd = new MySqlCommand(@"
            SELECT p.id_passeio, p.nome_passeio, p.descricao, p.img_url, COUNT(c.id_passeio) AS curtidas
            FROM passeio p
            LEFT JOIN curtida_passeio c ON p.id_passeio = c.id_passeio
            GROUP BY p.id_passeio
            ORDER BY curtidas DESC
            LIMIT 5", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Passeio
                        {
                            id_passeio = !reader.IsDBNull(reader.GetOrdinal("id_passeio")) ? reader.GetInt32("id_passeio") : 0,
                            nome_passeio = !reader.IsDBNull(reader.GetOrdinal("nome_passeio")) ? reader.GetString("nome_passeio") : "",
                            descricao = !reader.IsDBNull(reader.GetOrdinal("descricao")) ? reader.GetString("descricao") : "",
                            img_url = !reader.IsDBNull(reader.GetOrdinal("img_url")) ? reader.GetString("img_url") : null,
                            QuantidadeCurtidas = !reader.IsDBNull(reader.GetOrdinal("curtidas")) ? reader.GetInt32("curtidas") : 0
                        });
                    }

                }
            }

            return lista;
        }
    }
}
