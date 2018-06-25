using EntityTypeConfiguration.Entities;
using EntityTypeConfiguration.Mappings;
using Microsoft.EntityFrameworkCore;

namespace EntityTypeConfiguration
{
    class Program
    {
        static void Main(string[] args) { }

        public class LivrosContext : DbContext
        {
            public DbSet<Livro> Livro { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCore.Demo;Trusted_Connection=True;");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new LivroMap());
                modelBuilder.ApplyConfiguration(new LivroDetalhesMap());
            }
        }
    }
}
