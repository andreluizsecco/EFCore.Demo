using DbContextPooling.Models;
using Microsoft.EntityFrameworkCore;

namespace DbContextPooling.DbContextPooling
{
    public class LivrosContext : DbContext
    {
        public LivrosContext(DbContextOptions options) : base(options) {}

        public DbSet<Livro> Livros { get; set; }
    }
}