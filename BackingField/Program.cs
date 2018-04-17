using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BackingField
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new LivrosContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                if (db.Livros.Any())
                {
                    db.Database.ExecuteSqlCommand("DELETE FROM dbo.Livros");
                }

                var livro = new Livro { Autor = "Eric Evans", AnoPublicacao = 2003 };
                livro.SetTitulo("Domain - Driven Design: Tackling Complexity in the Heart of Software");

                db.Livros.Add(livro);

                db.SaveChanges();

                Console.WriteLine("------------ RESULTADOS ------------");
                db.Livros.ForEachAsync(x => Console.WriteLine("Título: " + x.GetTitulo()));
                Console.WriteLine("Livros encontrados: " + db.Livros.Count(x => EF.Property<string>(x, "Titulo").Contains("Domain")));
            }
        }

        public class LivrosContext : DbContext
        {
            public DbSet<Livro> Livros { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                // Connection resiliency
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCore.Demo;Trusted_Connection=True;",
                                            options => options.EnableRetryOnFailure());
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Livro>()
                    .Property<string>("Titulo")
                    .HasField("_titulo");
            }
        }

        public class Livro
        {
            private string _titulo;

            public int LivroId { get; set; }
            public string Autor { get; set; }
            public int AnoPublicacao { get; set; }

            public string GetTitulo() { return _titulo; }

            public void SetTitulo(string titulo)
            {
                _titulo = $"{titulo} - {Autor}";
            }
        }
    }
}
