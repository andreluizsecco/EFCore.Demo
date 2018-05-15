using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ShadowProperties
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new LivrosContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var livro1 = new Livro { Titulo = "Domain - Driven Design: Tackling Complexity in the Heart of Software", Autor = "Eric Evans", AnoPublicacao = 2003 };
                var livro2 = new Livro { Titulo = "Agile Principles, Patterns, and Practices in C#", Autor = "Robert C. Martin", AnoPublicacao = 2006 };
                
                db.Livros.AddRange(livro1, livro2);

                // Definindo o valor em cada objeto
                db.Entry(livro1).Property("UltimaAtualizacao").CurrentValue = DateTime.Now;
                db.Entry(livro2).Property("UltimaAtualizacao").CurrentValue = DateTime.Now.AddMinutes(1);
                
                db.SaveChanges();

                Console.WriteLine("------------ RESULTADOS ------------");
                db.Livros
                    .OrderBy(x => EF.Property<DateTime>(x, "UltimaAtualizacao"))
                    .Select(x => new { x.Titulo, UltimaAtualizacao = EF.Property<DateTime>(x, "UltimaAtualizacao") }).ToList()
                    .ForEach(x => Console.WriteLine($"Titulo: {x.Titulo} | Atualizado em {x.UltimaAtualizacao}"));
            }
        }

        public class LivrosContext : DbContext
        {
            public DbSet<Livro> Livros { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCore.Demo;Trusted_Connection=True;");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Definindo a shadow property com valor padrão baseado na função getdate do SQL Server
                modelBuilder.Entity<Livro>()
                    .Property<DateTime>("UltimaAtualizacao")
                    .HasDefaultValueSql("getdate()");
            }
        }

        public class Livro
        {
            public int LivroId { get; set; }
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public int AnoPublicacao { get; set; }
        }
    }
}
