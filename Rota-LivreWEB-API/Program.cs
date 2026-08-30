using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.Interfaces;
using Rota_LivreWEB_API.Repositories;
using Rota_LivreWEB_API.Services;
using Rota_LivreWEB_API.Hubs;
using System.Text;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// Controllers / MVC
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

// DbContext - PostgreSQL (Supabase)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Cache / Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
// JWT

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("JWT ERROR: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("TOKEN VALIDADO");
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

// Dependency Injection
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<PasseioRepository>();
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<EnderecoRepository>();
builder.Services.AddScoped<IPasseioService, PasseioService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddSignalR();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true) 
              .AllowCredentials()
    );
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("PainelAdminPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://rotalivre-admin.approtalivre.workers.dev") // URL do Cloudflare
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                       ForwardedHeaders.XForwardedProto
});


app.MapOpenApi(); 
app.MapScalarApiReference(); 


// Middleware pipeline
app.UseCors("AllowAll");

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseCors("PainelAdminPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // APIs

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapHub<GrupoHub>("/grupohub");

app.MapGet("/status-deploy", () => new { 
    status = "Online", 
    versao = "1.0.1", 
    data = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
});

app.Run();
