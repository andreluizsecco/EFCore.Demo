using System;

namespace EntityTypeConfiguration.Entities
{
    public class Livro
    {
        public int LivroId { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public int AnoPublicacao { get; set; }
        public DateTime DataCadastro { get; set; }
        public virtual LivroDetalhes Detalhes { get; set; }
    }
}