using Microsoft.EntityFrameworkCore;
using final_project.Models;

namespace final_project.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para las tablas
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<Presupuesto> Presupuestos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración adicional de precisión para campos decimales
            modelBuilder.Entity<Gasto>()
                .Property(g => g.Monto)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Presupuesto>()
                .Property(p => p.MontoTotal)
                .HasPrecision(18, 2);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Configurar para usar timestamp without time zone en PostgreSQL
            configurationBuilder.Properties<DateTime>()
                .HaveColumnType("timestamp without time zone");
        }
    }
}