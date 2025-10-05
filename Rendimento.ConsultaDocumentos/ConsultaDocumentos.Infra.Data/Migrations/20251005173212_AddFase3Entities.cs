using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultaDocumentos.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFase3Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoPessoa = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DataConsulta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataConsultaValidade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Inscricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NaturezaJuridica = table.Column<int>(type: "int", nullable: true),
                    DescricaoNaturezaJuridica = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Segmento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RamoAtividade = table.Column<int>(type: "int", nullable: true),
                    DescricaoRamoAtividade = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NomeFantasia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MatrizFilialQtde = table.Column<int>(type: "int", nullable: true),
                    Matriz = table.Column<bool>(type: "bit", nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NomeMae = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Sexo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TituloEleitor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ResidenteExterior = table.Column<bool>(type: "bit", nullable: true),
                    AnoObito = table.Column<int>(type: "int", nullable: true),
                    DataSituacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdSituacao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CodControle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DataFundacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrigemBureau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IdNacionalidade = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documento_Nacionalidade_IdNacionalidade",
                        column: x => x.IdNacionalidade,
                        principalTable: "Nacionalidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Documento_SituacaoCadastral_IdSituacao",
                        column: x => x.IdSituacao,
                        principalTable: "SituacaoCadastral",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Email",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDocumento = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnderecoEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Email", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Email_Documento_IdDocumento",
                        column: x => x.IdDocumento,
                        principalTable: "Documento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Endereco",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDocumento = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Logradouro = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Complemento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Bairro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CEP = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UF = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    TipoLogradouro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endereco", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Endereco_Documento_IdDocumento",
                        column: x => x.IdDocumento,
                        principalTable: "Documento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuadroSocietario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDocumento = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CPFSocio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NomeSocio = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    QualificacaoSocio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CpfCnpj = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Qualificacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CpfRepresentanteLegal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NomeRepresentanteLegal = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    QualificacaoRepresentanteLegal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataEntrada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataSaida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PercentualCapital = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuadroSocietario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuadroSocietario_Documento_IdDocumento",
                        column: x => x.IdDocumento,
                        principalTable: "Documento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Telefone",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDocumento = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DDD = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Numero = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telefone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Telefone_Documento_IdDocumento",
                        column: x => x.IdDocumento,
                        principalTable: "Documento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documento_DataConsultaValidade",
                table: "Documento",
                column: "DataConsultaValidade");

            migrationBuilder.CreateIndex(
                name: "IX_Documento_IdNacionalidade",
                table: "Documento",
                column: "IdNacionalidade");

            migrationBuilder.CreateIndex(
                name: "IX_Documento_IdSituacao",
                table: "Documento",
                column: "IdSituacao");

            migrationBuilder.CreateIndex(
                name: "IX_Documento_Numero",
                table: "Documento",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Email_EnderecoEmail",
                table: "Email",
                column: "EnderecoEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Email_IdDocumento",
                table: "Email",
                column: "IdDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_Endereco_CEP",
                table: "Endereco",
                column: "CEP");

            migrationBuilder.CreateIndex(
                name: "IX_Endereco_Cidade_UF",
                table: "Endereco",
                columns: new[] { "Cidade", "UF" });

            migrationBuilder.CreateIndex(
                name: "IX_Endereco_IdDocumento",
                table: "Endereco",
                column: "IdDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_QuadroSocietario_IdDocumento",
                table: "QuadroSocietario",
                column: "IdDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_Telefone_DDD",
                table: "Telefone",
                column: "DDD");

            migrationBuilder.CreateIndex(
                name: "IX_Telefone_IdDocumento",
                table: "Telefone",
                column: "IdDocumento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Email");

            migrationBuilder.DropTable(
                name: "Endereco");

            migrationBuilder.DropTable(
                name: "QuadroSocietario");

            migrationBuilder.DropTable(
                name: "Telefone");

            migrationBuilder.DropTable(
                name: "Documento");
        }
    }
}
