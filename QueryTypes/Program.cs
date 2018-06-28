using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QueryTypes
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new LivrosContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Database.ExecuteSqlCommand(
                    @"CREATE VIEW [dbo].[View_LivrosPorAutor] AS
                      SELECT Autor, COUNT(LivroId) AS QtdLivros 
                      FROM dbo.Livros
                      GROUP BY Autor"
                );

                db.Livros.Add(new Livro { Titulo = "Domain-Driven Design: Tackling Complexity in the Heart of Software", Autor = "Eric Evans", AnoPublicacao = 2003 });
                db.Livros.Add(new Livro { Titulo = "Agile Principles, Patterns, and Practices in C#", Autor = "Robert C. Martin", AnoPublicacao = 2006 });
                db.Livros.Add(new Livro { Titulo = "Clean Code: A Handbook of Agile Software Craftsmanship", Autor = "Robert C. Martin", AnoPublicacao = 2008 });
                db.Livros.Add(new Livro { Titulo = "Implementing Domain-Driven Design", Autor = "Vaughn Vernon", AnoPublicacao = 2013 });
                db.Livros.Add(new Livro { Titulo = "Patterns, Principles, and Practices of Domain-Driven Design", Autor = "Scott Millet", AnoPublicacao = 2015 });
                db.Livros.Add(new Livro { Titulo = "Refactoring: Improving the Design of Existing Code ", Autor = "Martin Fowler", AnoPublicacao = 2012 });

                db.SaveChanges();

                Console.WriteLine("------------ RESULTADOS - VIEW ------------");
                
                var livrosPorAutor = db.LivrosPorAutores.ToList();
                
                livrosPorAutor.ToList().ForEach(x =>
                {
                    Console.WriteLine($"Autor: {x.Autor}");
                    Console.WriteLine($"Quantidade de Livros: {x.QtdLivros}\r\n");
                });

                Console.WriteLine("------------ RESULTADOS - RAW SQL QUERIES ------------");

                var livrosPorAutorRawSql = db.LivrosPorAutores
                    .FromSql(@"SELECT Autor, COUNT(LivroId) AS QtdLivros 
                               FROM dbo.Livros
                               GROUP BY Autor");

                livrosPorAutor.ToList().ForEach(x =>
                {
                    Console.WriteLine($"Autor: {x.Autor}");
                    Console.WriteLine($"Quantidade de Livros: {x.QtdLivros}\r\n");
                });
            }
        }
    }

    public class LivrosContext : DbContext
    {
        public DbSet<Livro> Livros { get; set; }

        public DbQuery<LivrosPorAutor> LivrosPorAutores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=EFCore.Demo;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Query<LivrosPorAutor>().ToView("View_LivrosPorAutor");
        }
    }

    public class Livro
    {
        public int LivroId { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public int AnoPublicacao { get; set; }
    }

    public class LivrosPorAutor
    {
        public string Autor { get; set; }
        public int QtdLivros { get; set; }
    }
}
