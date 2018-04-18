using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ValueConversions
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new LivrosContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Pedidos.Add(new Pedido()
                {
                    Data = DateTime.Now,
                    Valor = 5000,
                    Cliente = new Cliente(){ Nome = "André Secco" },
                    Produtos = new List<Produto>()
                    {
                        new Produto(){ Nome = "Notebook" },
                        new Produto(){ Nome = "Celular" }
                    },
                    Status = StatusPedido.Recebido
                });

                db.SaveChanges();

                Console.WriteLine("------------ RESULTADOS ------------");
                db.Pedidos.ToList().ForEach(pedido => {
                    Console.WriteLine($"Cliente: {pedido.Cliente.Nome}");
                    Console.WriteLine($"Data: {pedido.Data}");
                    Console.WriteLine($"------ Produtos ------");
                    pedido.Produtos.ToList().ForEach(produto => 
                    {
                        Console.WriteLine($"{produto.Nome}");
                    });
                    Console.WriteLine();
                });
            }
        }
    }

    public class LivrosContext : DbContext
    {
        public DbSet<Pedido> Pedidos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCore.Demo;Trusted_Connection=True;")
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany();

            modelBuilder.Entity<Pedido>()
                .HasMany(p => p.Produtos)
                .WithOne();

            modelBuilder
                .Entity<Pedido>()
                .Property(e => e.Status)
                .HasConversion<string>();
        }
    }

    public class Pedido
    {
        public int PedidoID { get; set; }
        public DateTime Data { get; set; }
        public decimal  Valor { get; set; }
        public StatusPedido Status { get; set; }

        public virtual Cliente Cliente { get; set; }
        public virtual ICollection<Produto> Produtos { get; set; }
    }

    public class Cliente
    {
        public int ClienteID { get; set; }
        public string Nome { get; set; }
    }

    public class Produto
    {
        public int ProdutoID { get; set; }
        public string Nome { get; set; }
    }

    public enum StatusPedido
    {
        Recebido,
        Faturado,
        EmTransito,
        Enviado,
        Entregue
    }
}
