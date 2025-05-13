using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.DbContext;
using System.Linq;
using Rota_LivreWEB_API.Repositories;
using MySql.Data.MySqlClient;


namespace Rota_LivreWEB_API.Controllers
{
    public class PasseiosController : Controller
    {


        public ActionResult Categoria(int id)
        {
            var passeios = PasseioDb.BuscarPasseiosPorCategoria(id);

            return View(passeios);
        }

        public ActionResult Detalhes(int id)
        {
            var repo = new PasseioRepository();
            var passeio = repo.ObterPasseioPorId(id);
            if (passeio == null)
            {
                return NotFound();
            }

        
     
            return View(passeio);
        }



        

        
        [HttpPost]
        public ActionResult Curtir(int idPasseio)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return Json(new { sucesso = false, mensagem = "Usuário não autenticado." });
            }

            string connectionString = "Server=rotalivre.c30u6uc8o0pe.us-east-2.rds.amazonaws.com;Port=3306;Database=rotalivre;Uid=admin;Pwd=$Rotalivre1;";

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM curtida_passeio WHERE id_passeio = @idPasseio AND id_usuario = @idUsuario", conn);
                checkCmd.Parameters.AddWithValue("@idPasseio", idPasseio);
                checkCmd.Parameters.AddWithValue("@idUsuario", idUsuario.Value);

                int existe = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (existe == 0)
                {
                    var insertCmd = new MySqlCommand("INSERT INTO curtida_passeio (id_passeio, id_usuario) VALUES (@idPasseio, @idUsuario)", conn);
                    insertCmd.Parameters.AddWithValue("@idPasseio", idPasseio);
                    insertCmd.Parameters.AddWithValue("@idUsuario", idUsuario.Value);
                    insertCmd.ExecuteNonQuery();
                }

                var countCmd = new MySqlCommand("SELECT COUNT(*) FROM curtida_passeio WHERE id_passeio = @idPasseio", conn);
                countCmd.Parameters.AddWithValue("@idPasseio", idPasseio);
                int totalCurtidas = Convert.ToInt32(countCmd.ExecuteScalar());

                return Json(new { sucesso = true, curtidas = totalCurtidas });
            }
        }


    }

}
