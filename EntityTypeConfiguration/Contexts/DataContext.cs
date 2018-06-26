using EntityTypeConfiguration.Entities;
using EntityTypeConfiguration.Mappings;
using Microsoft.EntityFrameworkCore;

namespace EntityTypeConfiguration.Contexts
{
    public class DataContext : DbContext
    {
        public DbSet<Livro> Livros { get; set; }

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