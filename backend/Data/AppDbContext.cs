
using Estacionamento.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Estacionamento.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Vaga> Vagas => Set<Vaga>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Vaga>(entity =>
            {
                entity.Property(p => p.ValorCobrado).HasPrecision(10, 2);
                entity.Property(p => p.Placa).HasMaxLength(10).IsRequired();
                entity.Property(p => p.Marca).HasMaxLength(60).IsRequired();
                entity.Property(p => p.Modelo).HasMaxLength(60).IsRequired();
            });
        }
    }
}
