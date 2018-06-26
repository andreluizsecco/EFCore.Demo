using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityTypeConfiguration.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Livro",
                columns: table => new
                {
                    LivroId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Titulo = table.Column<string>(type: "varchar(200)", nullable: true),
                    Autor = table.Column<string>(type: "varchar(100)", nullable: true),
                    AnoPublicacao = table.Column<int>(nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livro", x => x.LivroId);
                });

            migrationBuilder.CreateTable(
                name: "LivroDetalhe",
                columns: table => new
                {
                    LivroId = table.Column<int>(nullable: false),
                    Editora = table.Column<string>(type: "varchar(100)", nullable: true),
                    NumeroPaginas = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivroDetalhe", x => x.LivroId);
                    table.ForeignKey(
                        name: "FK_LivroDetalhe_Livro_LivroId",
                        column: x => x.LivroId,
                        principalTable: "Livro",
                        principalColumn: "LivroId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LivroDetalhe");

            migrationBuilder.DropTable(
                name: "Livro");
        }
    }
}
