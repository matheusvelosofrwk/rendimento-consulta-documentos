using ConsultaDocumentos.Domain.Base;

namespace ConsultaDocumentos.Domain.Entities
{
    public class LogErro : BaseEntity
    {
        // Campos principais
        public DateTime DataHora { get; private set; }
        public string? Aplicacao { get; private set; }
        public string? Metodo { get; private set; }
        public string? Erro { get; private set; }
        public string? StackTrace { get; private set; }
        public string? Usuario { get; private set; }
        public Guid? IdSistema { get; private set; }

        // Navigation Properties
        public virtual Aplicacao? Sistema { get; private set; }

        // Construtor privado para EF Core
        private LogErro() { }

        // Factory Method
        public static LogErro Criar(
            DateTime dataHora,
            string? aplicacao = null,
            string? metodo = null,
            string? erro = null,
            string? stackTrace = null,
            string? usuario = null,
            Guid? idSistema = null)
        {
            return new LogErro
            {
                Id = Guid.NewGuid(),
                DataHora = dataHora,
                Aplicacao = aplicacao,
                Metodo = metodo,
                Erro = erro,
                StackTrace = stackTrace,
                Usuario = usuario,
                IdSistema = idSistema
            };
        }

        // Métodos de domínio
        public bool TemStackTrace() => !string.IsNullOrWhiteSpace(StackTrace);

        public bool TemUsuario() => !string.IsNullOrWhiteSpace(Usuario);

        public bool TemSistemaAssociado() => IdSistema.HasValue;

        public string GetResumoErro()
        {
            if (string.IsNullOrWhiteSpace(Erro))
                return "Erro sem descrição";

            return Erro.Length > 100 ? Erro.Substring(0, 97) + "..." : Erro;
        }
    }
}
