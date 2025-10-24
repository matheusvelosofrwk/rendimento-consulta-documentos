using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultaDocumentos.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoPessoaToSituacaoCadastral : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoPessoa",
                table: "SituacaoCadastral",
                type: "char(1)",
                nullable: false,
                defaultValue: "A");

            migrationBuilder.CreateIndex(
                name: "IX_SituacaoCadastral_TipoPessoa",
                table: "SituacaoCadastral",
                column: "TipoPessoa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SituacaoCadastral_TipoPessoa",
                table: "SituacaoCadastral");

            migrationBuilder.DropColumn(
                name: "TipoPessoa",
                table: "SituacaoCadastral");
        }
    }
}
