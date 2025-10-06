using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultaDocumentos.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposLegadosSistema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodPaisOrigem",
                table: "QuadroSocietario",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdNacionalidade",
                table: "QuadroSocietario",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomePaisOrigem",
                table: "QuadroSocietario",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "QuadroSocietario",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiaCorte",
                table: "Provedor",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dominio",
                table: "Provedor",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EndCertificado",
                table: "Provedor",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Porta",
                table: "Provedor",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QtdAcessoMaximo",
                table: "Provedor",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QtdAcessoMinimo",
                table: "Provedor",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QtdDiasValidadeEND",
                table: "Provedor",
                type: "int",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.AddColumn<int>(
                name: "QtdDiasValidadePF",
                table: "Provedor",
                type: "int",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.AddColumn<int>(
                name: "QtdDiasValidadePJ",
                table: "Provedor",
                type: "int",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.AddColumn<int>(
                name: "QtdMinEmailLog",
                table: "Provedor",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Senha",
                table: "Provedor",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoWebService",
                table: "Provedor",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<string>(
                name: "Usuario",
                table: "Provedor",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeSocial",
                table: "Documento",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Porte",
                table: "Documento",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataConsulta",
                table: "AplicacaoProvedor",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnderecoIP",
                table: "AplicacaoProvedor",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdDocumento",
                table: "AplicacaoProvedor",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemoteHost",
                table: "AplicacaoProvedor",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Serpro",
                table: "Aplicacao",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_QuadroSocietario_IdNacionalidade",
                table: "QuadroSocietario",
                column: "IdNacionalidade");

            migrationBuilder.CreateIndex(
                name: "IX_AplicacaoProvedor_Billing",
                table: "AplicacaoProvedor",
                columns: new[] { "AplicacaoId", "ProvedorId", "DataConsulta" });

            migrationBuilder.CreateIndex(
                name: "IX_AplicacaoProvedor_DataConsulta",
                table: "AplicacaoProvedor",
                column: "DataConsulta");

            migrationBuilder.CreateIndex(
                name: "IX_AplicacaoProvedor_IdDocumento",
                table: "AplicacaoProvedor",
                column: "IdDocumento");

            migrationBuilder.AddForeignKey(
                name: "FK_AplicacaoProvedor_Documento_IdDocumento",
                table: "AplicacaoProvedor",
                column: "IdDocumento",
                principalTable: "Documento",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_QuadroSocietario_Nacionalidade_IdNacionalidade",
                table: "QuadroSocietario",
                column: "IdNacionalidade",
                principalTable: "Nacionalidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AplicacaoProvedor_Documento_IdDocumento",
                table: "AplicacaoProvedor");

            migrationBuilder.DropForeignKey(
                name: "FK_QuadroSocietario_Nacionalidade_IdNacionalidade",
                table: "QuadroSocietario");

            migrationBuilder.DropIndex(
                name: "IX_QuadroSocietario_IdNacionalidade",
                table: "QuadroSocietario");

            migrationBuilder.DropIndex(
                name: "IX_AplicacaoProvedor_Billing",
                table: "AplicacaoProvedor");

            migrationBuilder.DropIndex(
                name: "IX_AplicacaoProvedor_DataConsulta",
                table: "AplicacaoProvedor");

            migrationBuilder.DropIndex(
                name: "IX_AplicacaoProvedor_IdDocumento",
                table: "AplicacaoProvedor");

            migrationBuilder.DropColumn(
                name: "CodPaisOrigem",
                table: "QuadroSocietario");

            migrationBuilder.DropColumn(
                name: "IdNacionalidade",
                table: "QuadroSocietario");

            migrationBuilder.DropColumn(
                name: "NomePaisOrigem",
                table: "QuadroSocietario");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "QuadroSocietario");

            migrationBuilder.DropColumn(
                name: "DiaCorte",
                table: "Provedor");

            migrationBuilder.DropColumn(
                name: "Dominio",
                table: "Provedor");

            migrationBuilder.DropColumn(
                name: "EndCertificado",
                table: "Provedor");

            migrationBuilder.DropColumn(
                name: "Porta",
                table: "Provedor");

            migrationBuilder.DropColumn(
                name: "QtdAcessoMaximo",
                table: "Provedor");

            migrationBuilder.DropColumn(
                name: "QtdAcessoMinimo",
                table: "Provedor");

            migrationBuilder.DropColumn(
                name: "QtdDiasValidadeEND",
                table: "Provedor");

            migrationBuilder.DropColumn(
                name: "QtdDiasValidadePF",
                table: "Provedor");

            migrationBuilder.DropColumn(
                name: "QtdDiasValidadePJ",
                table: "Provedor");

            migrationBuilder.DropColumn(
                name: "QtdMinEmailLog",
                table: "Provedor");

            migrationBuilder.DropColumn(
                name: "Senha",
                table: "Provedor");

            migrationBuilder.DropColumn(
                name: "TipoWebService",
                table: "Provedor");

            migrationBuilder.DropColumn(
                name: "Usuario",
                table: "Provedor");

            migrationBuilder.DropColumn(
                name: "NomeSocial",
                table: "Documento");

            migrationBuilder.DropColumn(
                name: "Porte",
                table: "Documento");

            migrationBuilder.DropColumn(
                name: "DataConsulta",
                table: "AplicacaoProvedor");

            migrationBuilder.DropColumn(
                name: "EnderecoIP",
                table: "AplicacaoProvedor");

            migrationBuilder.DropColumn(
                name: "IdDocumento",
                table: "AplicacaoProvedor");

            migrationBuilder.DropColumn(
                name: "RemoteHost",
                table: "AplicacaoProvedor");

            migrationBuilder.DropColumn(
                name: "Serpro",
                table: "Aplicacao");
        }
    }
}
