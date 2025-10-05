using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultaDocumentos.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLogAuditoriaAndLogErro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogAuditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AplicacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeAplicacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DocumentoNumero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoDocumento = table.Column<int>(type: "int", nullable: false),
                    ParametrosEntrada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProvedoresUtilizados = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProvedorPrincipal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConsultaSucesso = table.Column<bool>(type: "bit", nullable: false),
                    RespostaProvedor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MensagemRetorno = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TempoProcessamentoMs = table.Column<long>(type: "bigint", nullable: false),
                    DataHoraConsulta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EnderecoIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TokenAutenticacao = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    OrigemCache = table.Column<bool>(type: "bit", nullable: false),
                    InformacoesAdicionais = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogAuditoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogAuditoria_Aplicacao_AplicacaoId",
                        column: x => x.AplicacaoId,
                        principalTable: "Aplicacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogErro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aplicacao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Metodo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Erro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Usuario = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IdSistema = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogErro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogErro_Aplicacao_IdSistema",
                        column: x => x.IdSistema,
                        principalTable: "Aplicacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogAuditoria_AplicacaoId",
                table: "LogAuditoria",
                column: "AplicacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_LogAuditoria_AplicacaoId_DataHoraConsulta",
                table: "LogAuditoria",
                columns: new[] { "AplicacaoId", "DataHoraConsulta" });

            migrationBuilder.CreateIndex(
                name: "IX_LogAuditoria_ConsultaSucesso",
                table: "LogAuditoria",
                column: "ConsultaSucesso");

            migrationBuilder.CreateIndex(
                name: "IX_LogAuditoria_DataHoraConsulta",
                table: "LogAuditoria",
                column: "DataHoraConsulta");

            migrationBuilder.CreateIndex(
                name: "IX_LogAuditoria_DocumentoNumero",
                table: "LogAuditoria",
                column: "DocumentoNumero");

            migrationBuilder.CreateIndex(
                name: "IX_LogErro_Aplicacao",
                table: "LogErro",
                column: "Aplicacao");

            migrationBuilder.CreateIndex(
                name: "IX_LogErro_DataHora",
                table: "LogErro",
                column: "DataHora");

            migrationBuilder.CreateIndex(
                name: "IX_LogErro_IdSistema",
                table: "LogErro",
                column: "IdSistema");

            migrationBuilder.CreateIndex(
                name: "IX_LogErro_Usuario",
                table: "LogErro",
                column: "Usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogAuditoria");

            migrationBuilder.DropTable(
                name: "LogErro");
        }
    }
}
