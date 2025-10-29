using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultaDocumentos.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificadoFieldsToAplicacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CertificadoCaminho",
                table: "Aplicacao",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CertificadoDataExpiracao",
                table: "Aplicacao",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificadoSenha",
                table: "Aplicacao",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificadoSenhaCriptografada",
                table: "Aplicacao",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificadoCaminho",
                table: "Aplicacao");

            migrationBuilder.DropColumn(
                name: "CertificadoDataExpiracao",
                table: "Aplicacao");

            migrationBuilder.DropColumn(
                name: "CertificadoSenha",
                table: "Aplicacao");

            migrationBuilder.DropColumn(
                name: "CertificadoSenhaCriptografada",
                table: "Aplicacao");
        }
    }
}
