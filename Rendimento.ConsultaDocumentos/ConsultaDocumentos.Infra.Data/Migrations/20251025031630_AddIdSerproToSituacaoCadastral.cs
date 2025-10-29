using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultaDocumentos.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIdSerproToSituacaoCadastral : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdSerpro",
                table: "SituacaoCadastral",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SituacaoCadastral_IdSerpro",
                table: "SituacaoCadastral",
                column: "IdSerpro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SituacaoCadastral_IdSerpro",
                table: "SituacaoCadastral");

            migrationBuilder.DropColumn(
                name: "IdSerpro",
                table: "SituacaoCadastral");
        }
    }
}
