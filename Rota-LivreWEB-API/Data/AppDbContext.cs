using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Passeio> Passeio { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Endereco> Endereco { get; set; }
        public DbSet<Avaliacao> Avaliacao { get; set; }
        public DbSet<PerguntaSeguranca> PerguntaSeguranca { get; set; }
        public DbSet<CurtidaPasseio> CurtidaPasseio { get; set; }
        public DbSet<Funcionario> Funcionario { get; set; }
        public DbSet<Grupo> Grupo { get; set; }
        public DbSet<GrupoPasseio> GrupoPasseio { get; set; }
        public DbSet<GrupoUsuario> GrupoUsuario { get; set; }
        public DbSet<Localizacao> Localizacao { get; set; }
        public DbSet<PasseioFavorito> PasseioFavorito { get; set; }
        public DbSet<PasseioFuncionario> PasseioFuncionario { get; set; }
        public DbSet<UsuarioLocalizacao> UsuarioLocalizacao { get; set; }
        public DbSet<PasseioPendente> PasseioPendente { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.id_categoria);
            });

            modelBuilder.Entity<Endereco>(entity =>
            {
                entity.HasKey(e => e.id_endereco);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.id_usuario);
            });

            modelBuilder.Entity<Avaliacao>(entity =>
            {
                entity.HasKey(e => e.id_avaliacao);
            });

            modelBuilder.Entity<PerguntaSeguranca>(entity =>
            {
                entity.HasKey(e => e.id_pergunta);
            });

            modelBuilder.Entity<Passeio>(entity =>
            {
                entity.HasKey(e => e.id_passeio);
            });

            modelBuilder.Entity<Passeio>()
    .HasOne(p => p.Endereco)
    .WithMany()
    .HasForeignKey(p => p.Enderecoid_endereco)
    .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
