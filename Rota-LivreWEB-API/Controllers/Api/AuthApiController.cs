using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Repositories;

namespace Rota_LivreWEB_API.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthApiController : ControllerBase
{
    private readonly UsuarioRepository _usuarioRp;

    public AuthApiController(UsuarioRepository usuarioRp)
    {
        _usuarioRp = usuarioRp;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (_usuarioRp.VerificarLogin(request.Email, request.Senha))
        {
            var id = _usuarioRp.BuscarIdPorEmail(request.Email);
            var nome = _usuarioRp.BuscarNomePorEmail(request.Email);

            return Ok(new
            {
                Id = id,
                Nome = nome
            });
        }

        return Unauthorized(new { mensagem = "Email ou senha inválidos" });
    }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Senha { get; set; }
}