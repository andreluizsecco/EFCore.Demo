using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ModelLevelQueryFilters
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new LivrosContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Livros.Add(new Livro { Titulo = "Domain-Driven Design: Tackling Complexity in the Heart of Software", Autor = "Eric Evans", AnoPublicacao = 2003 });
                db.Livros.Add(new Livro { Titulo = "Agile Principles, Patterns, and Practices in C#", Autor = "Robert C. Martin", AnoPublicacao = 2006 });
                db.Livros.Add(new Livro { Titulo = "Clean Code: A Handbook of Agile Software Craftsmanship", Autor = "Robert C. Martin", AnoPublicacao = 2008 });
                db.Livros.Add(new Livro { Titulo = "Implementing Domain-Driven Design", Autor = "Vaughn Vernon", AnoPublicacao = 2013, DataExclusao = DateTime.Now });
                db.Livros.Add(new Livro { Titulo = "Patterns, Principles, and Practices of Domain-Driven Design", Autor = "Scott Millet", AnoPublicacao = 2015, DataExclusao = DateTime.Now });
                db.Livros.Add(new Livro { Titulo = "Refactoring: Improving the Design of Existing Code ", Autor = "Martin Fowler", AnoPublicacao = 2012, DataExclusao = DateTime.Now });

                db.SaveChanges();

                Console.WriteLine("------------ RESULTADOS ------------");
                db.Livros.ToList().ForEach(x => Console.WriteLine("Título: " + x.Titulo));
                System.Console.WriteLine();
                Console.WriteLine("------------ RESULTADOS (IGNORANDO QUERY FILTERS) ------------");
                db.Livros.IgnoreQueryFilters().ToList().ForEach(x => Console.WriteLine("Título: " + x.Titulo));
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
                modelBuilder.Entity<Livro>()
                    .HasQueryFilter(q => q.DataExclusao == null);
            }
        }

        public class Livro
        {
            public int LivroId { get; set; }
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public int AnoPublicacao { get; set; }
            public DateTime? DataExclusao { get; set; }
        }
    }
}
