using Microsoft.EntityFrameworkCore;
using ChamadoTrackerIA.Models;

namespace ChamadoTrackerIA.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Chamado> Chamados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Chamado>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Numero).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Assunto).HasMaxLength(500);
                entity.Property(e => e.Servico).HasMaxLength(200);
                entity.Property(e => e.Responsavel).HasMaxLength(200);
                entity.Property(e => e.AbertoEm).IsRequired();
            });
        }
    }
}

