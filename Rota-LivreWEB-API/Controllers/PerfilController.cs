using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.Repositories;

namespace Rota_LivreWEB_API.Controllers
{
    public class PerfilController : Controller
    {
        private readonly UsuarioRepository _usuarioRp;
        private readonly IConfiguration _config;

        public PerfilController(UsuarioRepository usuarioRp, IConfiguration config)
        {
            _usuarioRp = usuarioRp;
            _config = config;
        }

        [Authorize]
        [HttpGet("teste")]
        public IActionResult TesteProtegido()
        {
            return Ok("Você está autenticado!");
        }

        public async Task<ActionResult> Perfil()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            var usuario = await _usuarioRp.BuscarPorIdAsync(idUsuario.Value);
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFoto(IFormFile foto)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            var usuario = await _usuarioRp.BuscarPorIdAsync(idUsuario.Value);
            if (usuario == null) return NotFound();

            if (foto == null || foto.Length == 0)
            {
                TempData["ErroFoto"] = "Por favor, selecione uma imagem.";
                return RedirectToAction("Perfil");
            }

            var tiposPermitidos = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!tiposPermitidos.Contains(foto.ContentType.ToLower()))
            {
                TempData["ErroFoto"] = "Formato de imagem não suportado. Use JPG, PNG ou WEBP.";
                return RedirectToAction("Perfil");
            }

            var supabaseUrl = _config["Supabase:Url"];
            var supabaseKey = _config["Supabase:Key"];
            var bucket = _config["Supabase:Bucket"];

            var extensao = Path.GetExtension(foto.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extensao))
            {
                TempData["ErroFoto"] = "Não foi possível identificar a extensão da imagem.";
                return RedirectToAction("Perfil");
            }

            var fileName = $"usuario_{usuario.id_usuario}_{Guid.NewGuid()}{extensao}";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supabaseKey);
            httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);

            using var stream = foto.OpenReadStream();
            using var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue(foto.ContentType);

            var response = await httpClient.PostAsync($"{supabaseUrl}/storage/v1/object/{bucket}/{fileName}", content);
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErroFoto"] = "Erro ao enviar a imagem para o armazenamento.";
                return RedirectToAction("Perfil");
            }

            var urlPublica = $"{supabaseUrl}/storage/v1/object/public/{bucket}/{fileName}";
            var fotoAntiga = usuario.FotoPerfilUrl;
            usuario.FotoPerfilUrl = urlPublica;

            await _usuarioRp.AtualizarUsuarioAsync(usuario);

            if (!string.IsNullOrEmpty(fotoAntiga))
            {
                await RemoverArquivoSupabase(fotoAntiga);
            }

            TempData["SucessoFoto"] = "Foto de perfil atualizada com sucesso!";
            return RedirectToAction("Perfil");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverFoto()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            var usuario = await _usuarioRp.BuscarPorIdAsync(idUsuario.Value);
            if (usuario == null) return NotFound();

            if (!string.IsNullOrEmpty(usuario.FotoPerfilUrl))
            {
                await RemoverArquivoSupabase(usuario.FotoPerfilUrl);
                usuario.FotoPerfilUrl = null;
                await _usuarioRp.AtualizarUsuarioAsync(usuario);
                TempData["SucessoFoto"] = "Foto de perfil removida com sucesso!";
            }

            return RedirectToAction("Perfil");
        }

        private async Task<bool> RemoverArquivoSupabase(string urlFoto)
        {
            try
            {
                var uri = new Uri(urlFoto);
                var fileName = Path.GetFileName(uri.LocalPath);

                var supabaseUrl = _config["Supabase:Url"];
                var supabaseKey = _config["Supabase:Key"];
                var bucket = _config["Supabase:Bucket"];

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supabaseKey);
                client.DefaultRequestHeaders.Add("apikey", supabaseKey);

                var request = new HttpRequestMessage(
                    HttpMethod.Delete,
                    $"{supabaseUrl}/storage/v1/object/{bucket}/{fileName}"
                );

                var response = await client.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ActionResult> Editar()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            var usuario = await _usuarioRp.BuscarPorIdAsync(idUsuario.Value);
            if (usuario == null) return NotFound();

            var model = new UsuarioEdicaoViewModel
            {
                id_usuario = usuario.id_usuario,
                nome_completo = usuario.nome_completo,
                email = usuario.email,
                data_nasc = usuario.data_nasc
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(UsuarioEdicaoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model); 

            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            var usuario = await _usuarioRp.BuscarPorIdAsync(idUsuario.Value);
            if (usuario == null) return NotFound();

            usuario.nome_completo = model.nome_completo;
            usuario.email = model.email;
            usuario.data_nasc = model.data_nasc;

            await _usuarioRp.AtualizarUsuarioAsync(usuario);

            return RedirectToAction("Perfil");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Deletar()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario != null)
            {
                await _usuarioRp.DeletarUsuarioAsync(idUsuario.Value);
                HttpContext.Session.Clear();
            }

            return RedirectToAction("Login", "Login");
        }
    }
}
