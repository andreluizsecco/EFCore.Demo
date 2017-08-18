using InMemory.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace InMemory.DAL
{
    public class LivroContext : DbContext
    {
        public LivroContext(DbContextOptions options) : base(options) { }

        public LivroContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("InMemoryProvider");
        }


        public DbSet<Livro> Livros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Livro>().HasAlternateKey(c => c.Titulo);
        }

    }
}