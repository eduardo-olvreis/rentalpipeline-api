using Microsoft.EntityFrameworkCore;
using RentalPipeline.Entities;

namespace RentalPipeline.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Imovel> Imoveis => Set<Imovel>();
        public DbSet<Proposta> Propostas => Set<Proposta>();
        public DbSet<HistoricoProposta> HistoricoPropostas => Set<HistoricoProposta>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Cliente>(e =>
            {
                e.ToTable("clientes");
                e.HasKey(c => c.Id);
                e.HasIndex(c => c.Cpf).IsUnique();
            });

            modelBuilder.Entity<Imovel>(e =>
            {
                e.ToTable("imoveis");
                e.HasKey(i => i.Id);
                e.Property(i => i.Status).HasConversion<string>();
            });

            modelBuilder.Entity<Proposta>(e =>
            {
                e.ToTable("propostas");
                e.HasKey(p => p.Id);
                e.Property(p => p.Status).HasConversion<string>();
                e.HasOne(p => p.Imovel).WithMany().HasForeignKey(p => p.ImovelId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(p => p.Cliente).WithMany().HasForeignKey(p => p.ClienteId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<HistoricoProposta>(e =>
            {
                e.ToTable("historico_propostas");
                e.HasKey(h => h.Id);
                e.Property(h => h.StatusAnterior).HasConversion<string>();
                e.Property(h => h.StatusNovo).HasConversion<string>();
            });
        }
    }
}
