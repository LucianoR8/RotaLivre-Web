using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Rota_LivreWEB_API.Data; // Adicionado para acessar o banco diretamente

namespace Rota_LivreWEB_API.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthApiController : ControllerBase
{
    private readonly UsuarioRepository _usuarioRp;
    private readonly IConfiguration _config;
    private readonly AppDbContext _context; // Injeção do contexto do EF Core

    public AuthApiController(UsuarioRepository usuarioRp, IConfiguration config, AppDbContext context)
    {
        _usuarioRp = usuarioRp;
        _config = config;
        _context = context;
    }

    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginRequest request)
    {
        Console.WriteLine($"EMAIL: {request.Email}");
        Console.WriteLine($"SENHA: {request.Senha}");

        // Verifica a senha criptografada usando o seu repositório original
        var resultado = _usuarioRp.VerificarLogin(request.Email, request.Senha);

        Console.WriteLine($"LOGIN OK? {resultado}");

        if (!resultado)
            return Unauthorized(new { mensagem = "Email ou senha inválidos" });

        // Agora buscamos o usuário completo direto pelo EF Core para ter acesso ao is_admin
        var usuario = _context.Usuario.FirstOrDefault(u => u.email == request.Email);

        if (usuario == null)
            return Unauthorized(new { mensagem = "Usuário não encontrado." });

        Console.WriteLine($"ID: {usuario.id_usuario}");
        Console.WriteLine($"NOME: {usuario.nome_completo}");
        Console.WriteLine($"IS_ADMIN: {usuario.is_admin}");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["JwtSettings:Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.id_usuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.nome_completo),
                new Claim(ClaimTypes.Email, usuario.email),
                new Claim("IsAdmin", usuario.is_admin.ToString()) // Flag no token
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // Retorna o objeto exatamente como o React está esperando
        return Ok(new
        {
            sucesso = true,
            token = tokenString,
            usuario = new
            {
                Id = usuario.id_usuario,
                Nome = usuario.nome_completo,
                Email = usuario.email,
                IsAdmin = usuario.is_admin // React vai ler isso aqui!
            }
        });
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}