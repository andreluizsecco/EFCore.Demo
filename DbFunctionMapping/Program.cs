using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DbFunctionMapping
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
                    @"CREATE FUNCTION [dbo].[ExemplaresDisponiveis] (@livroId INT) RETURNS INT 
                      AS
                      BEGIN
                        DECLARE @count AS INT
                        SET @count = (SELECT QtdExemplares FROM Livros WHERE LivroId = @livroId)-(SELECT COUNT(*) FROM LivroEmprestimo WHERE LivroId = @livroId AND DataDevolucao IS NULL)
                        RETURN @count
                      END"
                );

                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "Domain-Driven Design: Tackling Complexity in the Heart of Software",
                        Autor = "Eric Evans",
                        AnoPublicacao = 2003,
                        QtdExemplares = 5,
                        Emprestimos = new List<LivroEmprestimo>()
                        {
                            new LivroEmprestimo()
                            {
                                NomeAluno = "José da Silva",
                                DataEmprestimo = DateTime.Parse("01/11/2017"),
                                DataLimiteDevolucao = DateTime.Parse("07/11/2017"),
                            },
                            new LivroEmprestimo()
                            {
                                NomeAluno = "Alexandre Alves",
                                DataEmprestimo = DateTime.Parse("03/11/2017"),
                                DataLimiteDevolucao = DateTime.Parse("10/11/2017"),
                            }
                        }
                    });

                db.Livros.Add(
                    new Livro 
                    {
                        Titulo = "Agile Principles, Patterns, and Practices in C#",
                        Autor = "Robert C. Martin",
                        AnoPublicacao = 2006,
                        QtdExemplares = 2,
                        Emprestimos = new List<LivroEmprestimo>()
                        {
                            new LivroEmprestimo()
                            {
                                NomeAluno = "Marcos Ribeiro",
                                DataEmprestimo = DateTime.Parse("02/11/2017"),
                                DataLimiteDevolucao = DateTime.Parse("08/11/2017"),
                            },
                            new LivroEmprestimo()
                            {
                                NomeAluno = "Fernando Lopes",
                                DataEmprestimo = DateTime.Parse("03/11/2017"),
                                DataLimiteDevolucao = DateTime.Parse("10/11/2017"),
                            }
                        }
                    });


                db.SaveChanges();

                Console.WriteLine("------------ RESULTADOS ------------");
                var query =
                    from p in db.Livros
                    select new 
                    {
                        Titulo =  p.Titulo,
                        ExemplaresDisponiveis = LivrosContext.ExemplaresDisponiveis(p.LivroId)
                    };

                query.ToList().ForEach(x => {
                    Console.WriteLine("Título: " + x.Titulo);
                    Console.WriteLine("Exemplares disponíveis: " + x.ExemplaresDisponiveis);
                });
                System.Console.WriteLine();
                Console.WriteLine("------------ RESULTADOS (Apenas disponíveis) ------------");
                var query2 =
                    from p in db.Livros
                    where LivrosContext.ExemplaresDisponiveis(p.LivroId) > 0
                    select p;

                query2.ToList().ForEach(x => {
                    Console.WriteLine("Título: " + x.Titulo);
                });
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
                    .HasMany(p => p.Emprestimos)
                    .WithOne(p => p.Livro);
            }

            [DbFunction(Schema="dbo", FunctionName="ExemplaresDisponiveis")]
            public static int ExemplaresDisponiveis(int livroId)
            {
                return 0;
            }
        }

        public class Livro
        {
            public int LivroId { get; set; }
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public int AnoPublicacao { get; set; }
            public int QtdExemplares { get; set; }
            public virtual ICollection<LivroEmprestimo> Emprestimos { get; set; }
        }

        public class LivroEmprestimo
        {
            public int LivroEmprestimoId { get; set; }
            public string NomeAluno { get; set; }
            public DateTime DataEmprestimo { get; set; }
            public DateTime DataLimiteDevolucao { get; set; }
            public DateTime? DataDevolucao { get; set; }
            public virtual Livro Livro { get; set; }
        }
    }
}
