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
    private readonly IConfiguration _config;

    public AuthApiController(UsuarioRepository usuarioRp, IConfiguration config)
    {
        _usuarioRp = usuarioRp;
        _config = config;
    }

    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginRequest request)
    {
        Console.WriteLine($"EMAIL: {request.Email}");
        Console.WriteLine($"SENHA: {request.Senha}");

        var resultado = _usuarioRp.VerificarLogin(request.Email, request.Senha);

        Console.WriteLine($"LOGIN OK? {resultado}");

        if (!resultado)
            return Unauthorized(new { mensagem = "Email ou senha inválidos" });

        var id = _usuarioRp.BuscarIdPorEmail(request.Email);
        var nome = _usuarioRp.BuscarNomePorEmail(request.Email);

        Console.WriteLine($"ID: {id}");
        Console.WriteLine($"NOME: {nome}");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["JwtSettings:Key"]);
        Console.WriteLine($"KEY: {_config["JwtSettings:Key"]}");

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