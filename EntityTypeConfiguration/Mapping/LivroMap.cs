using EntityTypeConfiguration.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityTypeConfiguration.Mappings
{
    public class LivroMap : IEntityTypeConfiguration<Livro>
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
}