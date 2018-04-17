using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace OwnedTypes
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new LivrosContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                
                db.Pessoas.Add(
                    new Pessoa { 
                        Nome = "André Luiz Secco",
                        Idade = 26,
                        Contato = new Contato() 
                        {
                            Telefone = new Telefone() { DDD = 49, Numero = 11111111 }
                        }  
                    });

                db.SaveChanges();

                var pessoa = db.Pessoas.FirstOrDefault();
                System.Console.WriteLine($"Nome: {pessoa.Nome}");
                System.Console.WriteLine($"Telefone: ({pessoa.Contato.Telefone.DDD}) {pessoa.Contato.Telefone.Numero}");
            }
        }

        public class LivrosContext : DbContext
        {
            public DbSet<Pessoa> Pessoas { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EFCore.Demo;Trusted_Connection=True;");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Pessoa>().OwnsOne(p => p.Contato, cb =>
                {
                    cb.OwnsOne(c => c.Telefone).ToTable("Telefone");
                });
            }
        }

        public class Pessoa
        {
            public int PessoaId { get; set; }
            public string Nome { get; set; }
            public byte Idade { get; set; }
            public virtual Contato Contato { get; set; }
        }

        public class Contato
        {
            public Telefone Telefone { get; set; }
        }

        public class Telefone
        {
            public byte DDD { get; set; }
            public long Numero { get; set; }
        }
    }
}
