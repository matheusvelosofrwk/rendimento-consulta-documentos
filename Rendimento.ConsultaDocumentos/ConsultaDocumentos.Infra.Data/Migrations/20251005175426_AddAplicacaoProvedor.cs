using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultaDocumentos.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAplicacaoProvedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AplicacaoProvedor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AplicacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProvedorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "char(1)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CriadoPor = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    AtualizadoPor = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AplicacaoProvedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AplicacaoProvedor_Aplicacao_AplicacaoId",
                        column: x => x.AplicacaoId,
                        principalTable: "Aplicacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AplicacaoProvedor_Provedor_ProvedorId",
                        column: x => x.ProvedorId,
                        principalTable: "Provedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AplicacaoProvedor_Aplicacao_Provedor",
                table: "AplicacaoProvedor",
                columns: new[] { "AplicacaoId", "ProvedorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AplicacaoProvedor_AplicacaoId",
                table: "AplicacaoProvedor",
                column: "AplicacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_AplicacaoProvedor_Ordem",
                table: "AplicacaoProvedor",
                column: "Ordem");

            migrationBuilder.CreateIndex(
                name: "IX_AplicacaoProvedor_ProvedorId",
                table: "AplicacaoProvedor",
                column: "ProvedorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AplicacaoProvedor");
        }
    }
}
