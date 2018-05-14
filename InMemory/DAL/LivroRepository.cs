using InMemory.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace InMemory.DAL
{
    public static class LivroRepository
    {
        static LivroRepository()
        {
            using (var context = new LivroContext())
            {
                AdicionarLivro(context, "Domain-Driven Design: Tackling Complexity in the Heart of Software", "Eric Evans", 2003);
                AdicionarLivro(context, "Agile Principles, Patterns, and Practices in C#", "Robert C. Martin", 2006);
                AdicionarLivro(context, "Clean Code: A Handbook of Agile Software Craftsmanship", "Robert C. Martin", 2008);
                AdicionarLivro(context, "Implementing Domain-Driven Design", "Vaughn Vernon", 2013);
                AdicionarLivro(context, "Patterns, Principles, and Practices of Domain-Driven Design", "Scott Millet", 2015);
                AdicionarLivro(context, "Refactoring: Improving the Design of Existing Code", "Martin Fowler", 2012);

                context.SaveChanges();
            }
        }

        private static void AdicionarLivro(LivroContext context, string titulo, string autor, int ano)
        {
            context.Livros.Add(new Livro()
                {
                    Titulo = titulo,
                    Autor = autor,
                    AnoPublicacao = ano
                });
        }

        public static List<Livro> ListarLivros()
        {
            using (var context = new LivroContext())
            {
                return context.Livros.OrderBy(c => c.Titulo).ToList();
            }
        }

        public static void ExcluirPrimeiroLivro()
        {
            using (var context = new LivroContext())
            {
                context.Livros.Remove(context.Livros.FirstOrDefault());
                context.SaveChanges();
            }
        }
    }
}