using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DbContextPooling.Models;
using DbContextPooling.DbContextPooling;

namespace DbContextPooling.Controllers
{
    public class HomeController : Controller
    {
        private readonly LivrosContext _db;

        public HomeController(LivrosContext db) =>
            _db = db;

        public IActionResult Index()
        {
            AdicionarLivros();
            return View(_db.Livros.ToList());
        }

        private void AdicionarLivros()
        {
            _db.Database.EnsureCreated();

            if (!_db.Livros.Any())
            {
                _db.Livros.Add(new Livro { Titulo = "Domain-Driven Design: Tackling Complexity in the Heart of Software", Autor = "Eric Evans", AnoPublicacao = 2003 });
                _db.Livros.Add(new Livro { Titulo = "Agile Principles, Patterns, and Practices in C#", Autor = "Robert C. Martin", AnoPublicacao = 2006 });
                _db.Livros.Add(new Livro { Titulo = "Clean Code: A Handbook of Agile Software Craftsmanship", Autor = "Robert C. Martin", AnoPublicacao = 2008 });
                _db.Livros.Add(new Livro { Titulo = "Implementing Domain-Driven Design", Autor = "Vaughn Vernon", AnoPublicacao = 2013 });
                _db.Livros.Add(new Livro { Titulo = "Patterns, Principles, and Practices of Domain-Driven Design", Autor = "Scott Millet", AnoPublicacao = 2015 });
                _db.Livros.Add(new Livro { Titulo = "Refactoring: Improving the Design of Existing Code ", Autor = "Martin Fowler", AnoPublicacao = 2012 });

                _db.SaveChanges();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
