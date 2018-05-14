using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FromSql
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

                db.Livros.Add(new Livro { Titulo = "Domain-Driven Design: Tackling Complexity in the Heart of Software", Autor = "Eric Evans", AnoPublicacao = 2003 });
                db.Livros.Add(new Livro { Titulo = "Agile Principles, Patterns, and Practices in C#", Autor = "Robert C. Martin", AnoPublicacao = 2006 });
                db.Livros.Add(new Livro { Titulo = "Clean Code: A Handbook of Agile Software Craftsmanship", Autor = "Robert C. Martin", AnoPublicacao = 2008 });
                db.Livros.Add(new Livro { Titulo = "Implementing Domain-Driven Design", Autor = "Vaughn Vernon", AnoPublicacao = 2013 });
                db.Livros.Add(new Livro { Titulo = "Patterns, Principles, and Practices of Domain-Driven Design", Autor = "Scott Millet", AnoPublicacao = 2015 });
                db.Livros.Add(new Livro { Titulo = "Refactoring: Improving the Design of Existing Code", Autor = "Martin Fowler", AnoPublicacao = 2012 });

                db.SaveChanges();

                System.Console.WriteLine("****** CONSULTA COMUM ******");
                ConsultaComum("Domain");

                System.Console.WriteLine("****** CONSULTA COM LINQ ******");
                ConsultaComLinq("Domain");

                System.Console.WriteLine("****** CONSULTA COM STRING INTERPOLATION ******");
                ConsultaComStringInterpolation("2013");

                System.Console.WriteLine("****** CONSULTA COM STRING INTERPOLATION - PROBLEMAS DE SQL INJECTION ******");
                ConsultaComStringInterpolationSqlInjection("2013; DROP TABLE Livros;");
            }
        }

        public static void ConsultaComum(string filtro)
        {
            using(var db = new LivrosContext())
            {
                var livros = db.Livros.FromSql("SELECT * FROM dbo.Livros WHERE Titulo LIKE '%' + @p0 + '%'", filtro).ToList();
                Console.WriteLine("------------ RESULTADOS ------------");
                livros.ForEach(x => Console.WriteLine("Título: " + x.Titulo));
            }
        }

        public static void ConsultaComLinq(string filtro)
        {
            using(var db = new LivrosContext())
            {
                var livros = db.Livros.FromSql("SELECT * FROM dbo.Livros WHERE Titulo LIKE '%' + @p0 + '%'", filtro).Where(x => x.AnoPublicacao == 2013).ToList();
                Console.WriteLine("------------ RESULTADOS ------------");
                livros.ForEach(x => Console.WriteLine("Título: " + x.Titulo));
            }
        }

        public static void ConsultaComStringInterpolation(string filtro)
        {
            using(var db = new LivrosContext())
            {
                var livros = db.Livros.FromSql($"SELECT * FROM dbo.Livros WHERE AnoPublicacao  = {filtro}").ToList();
                Console.WriteLine("------------ RESULTADOS ------------");
                livros.ForEach(x => Console.WriteLine("Título: " + x.Titulo));
            }
        }

        public static void ConsultaComStringInterpolationSqlInjection(string filtro)
        {
            using(var db = new LivrosContext())
            {
                var sql = $"SELECT * FROM dbo.Livros WHERE AnoPublicacao = {filtro}";
                var livros = db.Livros.FromSql(sql).ToList();
                Console.WriteLine("------------ RESULTADOS ------------");
                livros.ForEach(x => Console.WriteLine("Título: " + x.Titulo));
            }
        }

        public class LivrosContext : DbContext
        {
            public DbSet<Livro> Livros { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCore.Demo;Trusted_Connection=True;");
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
