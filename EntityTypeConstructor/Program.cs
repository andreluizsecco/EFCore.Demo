using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EntityTypeConstructor
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
                    new Livro("Domain-Driven Design: Tackling Complexity in the Heart of Software", "Eric Evans", 2003)
                );

                db.SaveChanges();

                Console.WriteLine("------------ RESULTADOS ------------");
                var livro = db.Livros.FirstOrDefault();
                Console.WriteLine($"Título: {livro.Titulo}");
                Console.WriteLine($"Autor: {livro.Autor}");
                Console.WriteLine($"Publicação: {livro.AnoPublicacao}");
            }
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
            modelBuilder.Entity<Livro>(
            livro =>
            {
                livro.Property(x => x.Titulo);
                livro.Property(x => x.Autor);
            });
        }
    }

    public class Livro
    {
        public Livro(string titulo, string autor, int anoPublicacao)
        {
            Autor = autor;
            Titulo = titulo;
            AnoPublicacao = anoPublicacao;
        }

        public int LivroId { get; set; }
        public string Titulo { get; }
        public string Autor { get; }
        public int AnoPublicacao { get; private set; }
    }
}
