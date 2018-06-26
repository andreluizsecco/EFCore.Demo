using EntityTypeConfiguration.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityTypeConfiguration.Mappings
{
    public class LivroDetalhesMap : IEntityTypeConfiguration<LivroDetalhes>
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
}