namespace EntityTypeConfiguration.Entities
{
    public class LivroDetalhes
    {
        public int LivroId { get; set; }
        public string Editora { get; set; }
        public int NumeroPaginas { get; set; }
        public virtual Livro Livro { get; set; }
    }
}