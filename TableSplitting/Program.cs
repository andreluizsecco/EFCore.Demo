using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace TableSplitting
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new LivrosContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                
                db.Livros.Add(
                    new Livro { 
                        Titulo = "Domain-Driven Design: Tackling Complexity in the Heart of Software",
                        Autor = "Eric Evans",
                        AnoPublicacao = 2003,
                        Detalhes = new LivroDetalhes() { Editora = "Addison-Wesley", NumeroPaginas = 560 }  
                    });

                db.SaveChanges();

                var livros = db.Livros.FirstOrDefault();
            }
        }

        public class LivrosContext : DbContext
        {
            public DbSet<Livro> Livros { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EFCore.Demo;Trusted_Connection=True;");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<LivroDetalhes>()
                    .HasKey(p => p.LivroId);

                modelBuilder.Entity<Livro>()
                    .HasOne(p => p.Detalhes).WithOne(p => p.Livro)
                    .HasForeignKey<LivroDetalhes>(e => e.LivroId);
                modelBuilder.Entity<Livro>().ToTable("Livros");
                modelBuilder.Entity<LivroDetalhes>().ToTable("Livros");
            }
        }

        public class Livro
        {
            public int LivroId { get; set; }
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public int AnoPublicacao { get; set; }
            public virtual LivroDetalhes Detalhes { get; set; }
        }

        public class LivroDetalhes
        {
            public int LivroId { get; set; }
            public string Editora { get; set; }
            public int NumeroPaginas { get; set; }
            public virtual Livro Livro { get; set; }
        }
    }
}
