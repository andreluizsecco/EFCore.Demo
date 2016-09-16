using InMemory.Models;
using Microsoft.EntityFrameworkCore;

namespace InMemory.DAL
{
    public class LivroContext : DbContext
    {
        public LivroContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Livro> Livros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Livro>().HasAlternateKey(c => c.Titulo);
        }

    }
}