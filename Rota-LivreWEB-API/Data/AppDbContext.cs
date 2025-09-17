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
        }

    }
}
