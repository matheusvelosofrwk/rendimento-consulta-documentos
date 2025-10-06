namespace ConsultaDocumentos.Application.DTOs.External
{
    public class ConsultaScoreResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string? ProvedorUtilizado { get; set; }
        public DateTime DataConsulta { get; set; }
        public ScoreDataDTO? Score { get; set; }
        public string? Erro { get; set; }
    }

    public class ScoreDataDTO
    {
        public int Valor { get; set; }
        public string? Classificacao { get; set; }
        public string? Faixa { get; set; }
        public DateTime? DataCalculo { get; set; }
        public List<string>? FatoresPositivos { get; set; }
        public List<string>? FatoresNegativos { get; set; }
    }
}
