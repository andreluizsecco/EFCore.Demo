using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityTypeConfiguration
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new LivrosContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
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
                modelBuilder.ApplyConfiguration(new LivroConfiguration());
                modelBuilder.ApplyConfiguration(new LivroDetalhesConfiguration());
            }
        }

        public class LivroConfiguration : IEntityTypeConfiguration<Livro>
        {
            public void Configure(EntityTypeBuilder<Livro> builder)
            {
                builder.ToTable("Livro");
                builder.HasKey(c => c.LivroId);
                builder.Property(c => c.Titulo)
                    .HasColumnType("varchar(200)");
                builder.Property(c => c.Autor)
                    .HasColumnType("varchar(100)");
                builder.Property(c => c.DataCadastro)
                    .HasColumnType("date");
            }
        }

        public class LivroDetalhesConfiguration : IEntityTypeConfiguration<LivroDetalhes>
        {
            public void Configure(EntityTypeBuilder<LivroDetalhes> builder)
            {
                builder.ToTable("LivroDetalhe");
                builder.HasKey(c => c.LivroId);
                builder.Property(c => c.Editora)
                    .HasColumnType("varchar(100)");
                builder.Property(c => c.NumeroPaginas)
                    .HasColumnType("smallint");
                builder.HasOne(p => p.Livro)
                    .WithOne(p => p.Detalhes);                
            }
        }

        public class Livro
        {
            public int LivroId { get; set; }
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public int AnoPublicacao { get; set; }
            public DateTime DataCadastro { get; set; }
            public virtual LivroDetalhes Detalhes { get; set; }
        }

        public class LivroDetalhes
        {
            public int LivroId { get; set; }
            public string Editora { get; set; }
            public int NumeroPaginas { get; set; }
            public virtual Livro Livro { get; set; }
        }
    }
}
