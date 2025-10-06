namespace ConsultaDocumentos.Application.Helpers
{
    public static class SituacaoCadastralMapper
    {
        private static readonly Dictionary<string, Guid> SituacoesCadastralMap = new()
        {
            // Pessoa Física - SERPRO
            { "ATIVA", Guid.Parse("00000000-0000-0000-0000-000000000001") },
            { "SUSPENSA", Guid.Parse("00000000-0000-0000-0000-000000000002") },
            { "INAPTA", Guid.Parse("00000000-0000-0000-0000-000000000003") },
            { "BAIXADA", Guid.Parse("00000000-0000-0000-0000-000000000004") },
            { "NULA", Guid.Parse("00000000-0000-0000-0000-000000000005") },
            { "CANCELADA", Guid.Parse("00000000-0000-0000-0000-000000000006") },

            // Pessoa Física - SERASA
            { "REGULAR", Guid.Parse("00000000-0000-0000-0000-000000000001") },
            { "SUSPENSO", Guid.Parse("00000000-0000-0000-0000-000000000002") },
            { "CANCELADO", Guid.Parse("00000000-0000-0000-0000-000000000003") },
            { "PENDENTE DE REGULARIZACAO", Guid.Parse("00000000-0000-0000-0000-000000000004") },
            { "NULO", Guid.Parse("00000000-0000-0000-0000-000000000005") }
        };

        public static Guid? MapearSituacao(string? situacaoCadastral)
        {
            if (string.IsNullOrWhiteSpace(situacaoCadastral))
                return null;

            var situacaoUpper = situacaoCadastral.Trim().ToUpperInvariant();

            if (SituacoesCadastralMap.TryGetValue(situacaoUpper, out var id))
            {
                return id;
            }

            // Retorna null se não encontrar mapeamento
            // O sistema poderá criar uma nova situação cadastral se necessário
            return null;
        }

        public static string? NormalizarSituacao(string? situacaoCadastral)
        {
            if (string.IsNullOrWhiteSpace(situacaoCadastral))
                return null;

            var situacaoUpper = situacaoCadastral.Trim().ToUpperInvariant();

            // Normaliza variações para o padrão SERPRO
            return situacaoUpper switch
            {
                "REGULAR" => "ATIVA",
                "SUSPENSO" => "SUSPENSA",
                "CANCELADO" => "CANCELADA",
                "NULO" => "NULA",
                _ => situacaoUpper
            };
        }

        public static bool IsAtiva(string? situacaoCadastral)
        {
            if (string.IsNullOrWhiteSpace(situacaoCadastral))
                return false;

            var situacaoNormalizada = NormalizarSituacao(situacaoCadastral);

            return situacaoNormalizada == "ATIVA" || situacaoNormalizada == "REGULAR";
        }
    }
}
