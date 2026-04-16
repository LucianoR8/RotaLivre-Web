using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
        if (!_usuarioRp.VerificarLogin(request.Email, request.Senha))
            return Unauthorized(new { mensagem = "Email ou senha inválidos" });

        var id = _usuarioRp.BuscarIdPorEmail(request.Email);
        var nome = _usuarioRp.BuscarNomePorEmail(request.Email);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("SUA_CHAVE_SUPER_SECRETA_AQUI_123456");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Name, nome),
            new Claim(ClaimTypes.Email, request.Email)
        }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new
        {
            token = tokenString,
            usuario = new
            {
                Id = id,
                Nome = nome
            }
        });
    }
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }

}