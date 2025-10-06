using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.DTOs.External;
using ConsultaDocumentos.Application.DTOs.External.Serasa;
using ConsultaDocumentos.Application.DTOs.External.Serpro;
using ConsultaDocumentos.Application.Exceptions;
using ConsultaDocumentos.Application.Helpers;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Enums;
using ConsultaDocumentos.Domain.Intefaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace ConsultaDocumentos.Application.Services.External
{
    public class ExternalDocumentConsultaService : IExternalDocumentConsultaService
    {
        private readonly ISerproService _serproService;
        private readonly ISerasaService _serasaService;
        private readonly IDocumentoService _documentoService;
        private readonly IAplicacaoProvedorRepository _aplicacaoProvedorRepository;
        private readonly IProvedorRepository _provedorRepository;
        private readonly ILogAuditoriaRepository _logAuditoriaRepository;
        private readonly ILogErroRepository _logErroRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly ILogger<ExternalDocumentConsultaService> _logger;

        public ExternalDocumentConsultaService(
            ISerproService serproService,
            ISerasaService serasaService,
            IDocumentoService documentoService,
            IAplicacaoProvedorRepository aplicacaoProvedorRepository,
            IProvedorRepository provedorRepository,
            ILogAuditoriaRepository logAuditoriaRepository,
            ILogErroRepository logErroRepository,
            ICacheService cacheService,
            IMapper mapper,
            ILogger<ExternalDocumentConsultaService> logger)
        {
            _serproService = serproService;
            _serasaService = serasaService;
            _documentoService = documentoService;
            _aplicacaoProvedorRepository = aplicacaoProvedorRepository;
            _provedorRepository = provedorRepository;
            _logAuditoriaRepository = logAuditoriaRepository;
            _logErroRepository = logErroRepository;
            _cacheService = cacheService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ConsultaDocumentoResponse> ConsultarDocumentoAsync(
            ConsultaDocumentoRequest request,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            var provedoresTentados = new List<string>();
            string? provedorUtilizado = null;
            bool origemCache = false;

            try
            {
                _logger.LogInformation(
                    "Iniciando consulta de documento {Tipo} {Numero} para aplicação {AplicacaoId}",
                    request.TipoDocumento, request.NumeroDocumento, request.AplicacaoId);

                // Verifica se existe em cache
                var documentoLimpo = DocumentoValidationHelper.RemoverFormatacao(request.NumeroDocumento);
                var cacheKey = $"documento:{documentoLimpo}";

                var documentoCacheDTO = await _cacheService.GetAsync<DocumentoDTO>(cacheKey, cancellationToken);
                if (documentoCacheDTO != null)
                {
                    _logger.LogInformation("Documento encontrado em cache válido");
                    stopwatch.Stop();

                    await RegistrarAuditoriaAsync(
                        request.AplicacaoId,
                        request.NumeroDocumento,
                        request.TipoDocumento,
                        true,
                        "CACHE",
                        "CACHE",
                        stopwatch.ElapsedMilliseconds,
                        true,
                        "Documento retornado do cache");

                    return new ConsultaDocumentoResponse
                    {
                        Sucesso = true,
                        Mensagem = "Documento retornado do cache",
                        ProvedorUtilizado = "CACHE",
                        ProvedoresTentados = new List<string> { "CACHE" },
                        OrigemCache = true,
                        TempoProcessamentoMs = stopwatch.ElapsedMilliseconds,
                        DataConsulta = DateTime.UtcNow,
                        Documento = documentoCacheDTO
                    };
                }

                // Busca provedores configurados para a aplicação
                var provedoresOrdenados = await ObterProvedoresOrdenadosAsync(request.AplicacaoId, cancellationToken);

                if (!provedoresOrdenados.Any())
                {
                    _logger.LogWarning("Nenhum provedor configurado para a aplicação {AplicacaoId}", request.AplicacaoId);
                    throw new ExternalProviderException("Nenhum provedor configurado para esta aplicação");
                }

                // Tenta consultar nos provedores conforme ordem de prioridade
                Documento? documentoConsultado = null;
                Exception? ultimoErro = null;

                foreach (var provedor in provedoresOrdenados)
                {
                    try
                    {
                        provedoresTentados.Add(provedor.Nome);
                        _logger.LogInformation("Tentando consulta no provedor {Provedor}", provedor.Nome);

                        documentoConsultado = await ConsultarNoProvedorAsync(
                            provedor.Nome,
                            request.NumeroDocumento,
                            request.TipoDocumento,
                            request.PerfilCNPJ,
                            cancellationToken);

                        if (documentoConsultado != null)
                        {
                            provedorUtilizado = provedor.Nome;
                            _logger.LogInformation("Consulta bem-sucedida no provedor {Provedor}", provedor.Nome);
                            break;
                        }
                    }
                    catch (ExternalProviderException ex)
                    {
                        ultimoErro = ex;
                        _logger.LogWarning(ex, "Falha ao consultar no provedor {Provedor}", provedor.Nome);

                        await RegistrarErroAsync(
                            request.AplicacaoId,
                            provedor.Nome,
                            ex.Message,
                            ex.ToString());

                        // Continua para o próximo provedor
                        continue;
                    }
                }

                stopwatch.Stop();

                if (documentoConsultado == null || provedorUtilizado == null)
                {
                    _logger.LogError("Falha em todos os provedores disponíveis");

                    await RegistrarAuditoriaAsync(
                        request.AplicacaoId,
                        request.NumeroDocumento,
                        request.TipoDocumento,
                        false,
                        string.Join(", ", provedoresTentados),
                        null,
                        stopwatch.ElapsedMilliseconds,
                        false,
                        ultimoErro?.Message ?? "Falha em todos os provedores");

                    return new ConsultaDocumentoResponse
                    {
                        Sucesso = false,
                        Mensagem = "Documento não encontrado em nenhum provedor disponível",
                        ProvedoresTentados = provedoresTentados,
                        TempoProcessamentoMs = stopwatch.ElapsedMilliseconds,
                        DataConsulta = DateTime.UtcNow,
                        Erro = ultimoErro?.Message ?? "Falha em todos os provedores"
                    };
                }

                // Salva o documento consultado
                var documentoSalvo = await SalvarDocumentoAsync(documentoConsultado, cancellationToken);
                var documentoDTOResult = _mapper.Map<DocumentoDTO>(documentoSalvo);

                // Atualiza o cache com o documento consultado
                await _cacheService.SetAsync(cacheKey, documentoDTOResult, TimeSpan.FromDays(90), cancellationToken);
                _logger.LogInformation("Documento armazenado no cache com chave: {CacheKey}", cacheKey);

                await RegistrarAuditoriaAsync(
                    request.AplicacaoId,
                    request.NumeroDocumento,
                    request.TipoDocumento,
                    true,
                    string.Join(", ", provedoresTentados),
                    provedorUtilizado,
                    stopwatch.ElapsedMilliseconds,
                    false,
                    "Documento consultado e salvo com sucesso");

                return new ConsultaDocumentoResponse
                {
                    Sucesso = true,
                    Mensagem = "Documento consultado com sucesso",
                    ProvedorUtilizado = provedorUtilizado,
                    ProvedoresTentados = provedoresTentados,
                    OrigemCache = false,
                    TempoProcessamentoMs = stopwatch.ElapsedMilliseconds,
                    DataConsulta = DateTime.UtcNow,
                    Documento = documentoDTOResult
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Erro inesperado ao consultar documento");

                await RegistrarErroAsync(
                    request.AplicacaoId,
                    "Sistema",
                    ex.Message,
                    ex.ToString());

                return new ConsultaDocumentoResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro ao processar consulta",
                    ProvedoresTentados = provedoresTentados,
                    TempoProcessamentoMs = stopwatch.ElapsedMilliseconds,
                    DataConsulta = DateTime.UtcNow,
                    Erro = ex.Message
                };
            }
        }

        public async Task<ConsultaScoreResponse> ConsultarScoreAsync(
            ConsultaScoreRequest request,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Consultando score de {Tipo} {Numero}",
                    request.TipoDocumento, request.NumeroDocumento);

                // Por enquanto, apenas SERASA oferece consulta de score
                var tipoDocumentoStr = request.TipoDocumento == TipoDocumento.CPF ? "CPF" : "CNPJ";

                var scoreResponse = await _serasaService.ConsultarScoreAsync(
                    request.NumeroDocumento,
                    tipoDocumentoStr,
                    cancellationToken);

                stopwatch.Stop();

                if (scoreResponse.Status != "SUCESSO" || scoreResponse.Score == null)
                {
                    return new ConsultaScoreResponse
                    {
                        Sucesso = false,
                        Mensagem = scoreResponse.Mensagem,
                        DataConsulta = DateTime.UtcNow,
                        ProvedorUtilizado = "SERASA",
                        Erro = scoreResponse.Mensagem
                    };
                }

                return new ConsultaScoreResponse
                {
                    Sucesso = true,
                    Mensagem = "Score consultado com sucesso",
                    ProvedorUtilizado = "SERASA",
                    DataConsulta = DateTime.UtcNow,
                    Score = new ScoreDataDTO
                    {
                        Valor = scoreResponse.Score.Valor ?? 0,
                        Classificacao = scoreResponse.Score.Classificacao,
                        Faixa = scoreResponse.Score.Faixa,
                        DataCalculo = DateConversionHelper.ParseSerasaDate(scoreResponse.Score.DataCalculo),
                        FatoresPositivos = scoreResponse.Score.FatoresPositivos,
                        FatoresNegativos = scoreResponse.Score.FatoresNegativos
                    }
                };
            }
            catch (ExternalProviderException ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Erro ao consultar score");

                return new ConsultaScoreResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro ao consultar score",
                    DataConsulta = DateTime.UtcNow,
                    ProvedorUtilizado = "SERASA",
                    Erro = ex.Message
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Erro inesperado ao consultar score");

                return new ConsultaScoreResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro inesperado ao consultar score",
                    DataConsulta = DateTime.UtcNow,
                    Erro = ex.Message
                };
            }
        }

        private async Task<List<Provedor>> ObterProvedoresOrdenadosAsync(Guid aplicacaoId, CancellationToken cancellationToken)
        {
            var aplicacaoProvedores = await _aplicacaoProvedorRepository.GetAllAsync();

            var provedoresAplicacao = aplicacaoProvedores
                .Where(ap => ap.AplicacaoId == aplicacaoId && ap.IsAtivo())
                .OrderBy(ap => ap.Ordem)
                .ToList();

            var provedores = new List<Provedor>();

            foreach (var ap in provedoresAplicacao)
            {
                var provedor = await _provedorRepository.GetByIdAsync(ap.ProvedorId);
                if (provedor != null && provedor.Status.ToUpperInvariant() == "ATIVO")
                {
                    provedores.Add(provedor);
                }
            }

            // Fallback: se não houver provedores configurados, usa SERPRO e SERASA na ordem padrão
            if (!provedores.Any())
            {
                var todosProvedores = await _provedorRepository.GetAllAsync();
                provedores = todosProvedores
                    .Where(p => p.Status.ToUpperInvariant() == "ATIVO")
                    .OrderBy(p => p.Prioridade)
                    .ToList();
            }

            return provedores;
        }

        private async Task<Documento?> ConsultarNoProvedorAsync(
            string nomeProvedor,
            string numeroDocumento,
            TipoDocumento tipoDocumento,
            int perfilCNPJ,
            CancellationToken cancellationToken)
        {
            switch (nomeProvedor.ToUpperInvariant())
            {
                case "SERPRO":
                    return await ConsultarSerproAsync(numeroDocumento, tipoDocumento, perfilCNPJ, cancellationToken);

                case "SERASA":
                    return await ConsultarSerasaAsync(numeroDocumento, tipoDocumento, cancellationToken);

                default:
                    _logger.LogWarning("Provedor {Provedor} não implementado", nomeProvedor);
                    throw new ExternalProviderException(nomeProvedor, "Provedor não implementado");
            }
        }

        private async Task<Documento?> ConsultarSerproAsync(
            string numeroDocumento,
            TipoDocumento tipoDocumento,
            int perfilCNPJ,
            CancellationToken cancellationToken)
        {
            if (tipoDocumento == TipoDocumento.CPF)
            {
                var response = await _serproService.ConsultarCPFAsync(numeroDocumento, cancellationToken);
                return ConverterSerprocCPFParaDocumento(response, numeroDocumento);
            }
            else
            {
                // Consulta CNPJ conforme perfil solicitado
                switch (perfilCNPJ)
                {
                    case 1:
                        var response1 = await _serproService.ConsultarCNPJPerfil1Async(numeroDocumento, cancellationToken);
                        return ConverterSerprocCNPJPerfil1ParaDocumento(response1, numeroDocumento);

                    case 2:
                        var response2 = await _serproService.ConsultarCNPJPerfil2Async(numeroDocumento, cancellationToken);
                        return ConverterSerprocCNPJPerfil2ParaDocumento(response2, numeroDocumento);

                    case 3:
                    default:
                        var response3 = await _serproService.ConsultarCNPJPerfil3Async(numeroDocumento, cancellationToken);
                        return ConverterSerprocCNPJPerfil3ParaDocumento(response3, numeroDocumento);
                }
            }
        }

        private async Task<Documento?> ConsultarSerasaAsync(
            string numeroDocumento,
            TipoDocumento tipoDocumento,
            CancellationToken cancellationToken)
        {
            if (tipoDocumento == TipoDocumento.CPF)
            {
                var response = await _serasaService.ConsultarCPFAsync(numeroDocumento, "COMPLETA", cancellationToken);
                return ConverterSerasaCPFParaDocumento(response, numeroDocumento);
            }
            else
            {
                var response = await _serasaService.ConsultarCNPJAsync(numeroDocumento, "COMPLETA", cancellationToken);
                return ConverterSerasaCNPJParaDocumento(response, numeroDocumento);
            }
        }

        // Métodos de conversão (SERPRO -> Documento)

        private Documento ConverterSerprocCPFParaDocumento(SerprocCPFResponse response, string numeroDocumento)
        {
            var documento = Documento.CriarPessoaFisica(
                DocumentoValidationHelper.RemoverFormatacao(numeroDocumento),
                response.Nome ?? string.Empty,
                DateConversionHelper.ParseSerproDate(response.DataNascimento) ?? DateTime.MinValue);

            documento.NomeMae = response.NomeMae;
            documento.Sexo = response.Sexo;
            documento.TituloEleitor = response.TituloEleitor;
            documento.ResidenteExterior = DateConversionHelper.ParseResidenteExterior(response.ResidenteExterior);
            documento.AnoObito = DateConversionHelper.ParseAnoObito(response.AnoObito);
            documento.CodControle = response.CodigoControle;
            documento.IdSituacao = SituacaoCadastralMapper.MapearSituacao(response.SituacaoCadastral);
            documento.OrigemBureau = "SERPRO";

            return documento;
        }

        private Documento ConverterSerprocCNPJPerfil1ParaDocumento(SerprocCNPJPerfil1Response response, string numeroDocumento)
        {
            var documento = Documento.CriarPessoaJuridica(
                DocumentoValidationHelper.RemoverFormatacao(numeroDocumento),
                response.NomeEmpresarial ?? string.Empty);

            documento.NomeFantasia = response.NomeFantasia;
            documento.Matriz = response.Estabelecimento?.ToUpperInvariant() == "MATRIZ";
            documento.IdSituacao = SituacaoCadastralMapper.MapearSituacao(response.SituacaoCadastral);
            documento.OrigemBureau = "SERPRO";

            return documento;
        }

        private Documento ConverterSerprocCNPJPerfil2ParaDocumento(SerprocCNPJPerfil2Response response, string numeroDocumento)
        {
            var documento = ConverterSerprocCNPJPerfil1ParaDocumento(response, numeroDocumento);

            documento.DataAbertura = DateConversionHelper.ParseSerproDate(response.DataAbertura);
            documento.DataSituacao = DateConversionHelper.ParseSerproDate(response.DataSituacaoCadastral);

            if (int.TryParse(response.NaturezaJuridica?.Replace("-", ""), out int naturezaJuridica))
            {
                documento.NaturezaJuridica = naturezaJuridica;
            }

            documento.DescricaoNaturezaJuridica = response.NaturezaJuridica;

            // TODO: Adicionar endereços, telefones e emails

            return documento;
        }

        private Documento ConverterSerprocCNPJPerfil3ParaDocumento(SerprocCNPJPerfil3Response response, string numeroDocumento)
        {
            var documento = ConverterSerprocCNPJPerfil2ParaDocumento(response, numeroDocumento);

            // Adiciona sócios
            if (response.Socios != null && response.Socios.Any())
            {
                foreach (var socioDTO in response.Socios)
                {
                    var socio = QuadroSocietario.Criar(
                        documento.Id,
                        socioDTO.CpfCnpj,
                        socioDTO.Nome,
                        socioDTO.Qualificacao);

                    socio.DataEntrada = DateConversionHelper.ParseSerproDate(socioDTO.DataEntrada);
                    socio.PercentualCapital = DecimalConversionHelper.ConvertToDecimal(socioDTO.PercentualCapital);
                    socio.CpfRepresentanteLegal = socioDTO.CpfRepresentanteLegal;
                    socio.NomeRepresentanteLegal = socioDTO.NomeRepresentanteLegal;
                    socio.QualificacaoRepresentanteLegal = socioDTO.QualificacaoRepresentanteLegal;

                    documento.AdicionarQuadroSocietario(socio);
                }
            }

            return documento;
        }

        // Métodos de conversão (SERASA -> Documento)

        private Documento ConverterSerasaCPFParaDocumento(SerasaCPFResponse response, string numeroDocumento)
        {
            if (response.Dados?.Identificacao == null)
            {
                throw new ExternalProviderException("SERASA", "Dados de identificação não retornados");
            }

            var documento = Documento.CriarPessoaFisica(
                DocumentoValidationHelper.RemoverFormatacao(numeroDocumento),
                response.Dados.Identificacao.Nome ?? string.Empty,
                DateConversionHelper.ParseSerasaDate(response.Dados.Identificacao.DataNascimento) ?? DateTime.MinValue);

            documento.NomeMae = response.Dados.Identificacao.NomeMae;
            documento.Sexo = response.Dados.Identificacao.Sexo;
            documento.IdSituacao = SituacaoCadastralMapper.MapearSituacao(response.Dados.Identificacao.SituacaoCadastral);
            documento.OrigemBureau = "SERASA";

            if (response.Dados.Documentos != null)
            {
                documento.TituloEleitor = response.Dados.Documentos.TituloEleitor;
            }

            // TODO: Adicionar endereços, telefones e emails

            return documento;
        }

        private Documento ConverterSerasaCNPJParaDocumento(SerasaCNPJResponse response, string numeroDocumento)
        {
            if (response.Dados?.Identificacao == null)
            {
                throw new ExternalProviderException("SERASA", "Dados de identificação não retornados");
            }

            var documento = Documento.CriarPessoaJuridica(
                DocumentoValidationHelper.RemoverFormatacao(numeroDocumento),
                response.Dados.Identificacao.RazaoSocial ?? string.Empty,
                DateConversionHelper.ParseSerasaDate(response.Dados.Identificacao.DataAbertura));

            documento.NomeFantasia = response.Dados.Identificacao.NomeFantasia;
            documento.DataSituacao = DateConversionHelper.ParseSerasaDate(response.Dados.Identificacao.DataSituacaoCadastral);
            documento.IdSituacao = SituacaoCadastralMapper.MapearSituacao(response.Dados.Identificacao.SituacaoCadastral);
            documento.OrigemBureau = "SERASA";

            if (response.Dados.Atividade != null)
            {
                if (int.TryParse(response.Dados.Atividade.NaturezaJuridica?.Replace("-", ""), out int naturezaJuridica))
                {
                    documento.NaturezaJuridica = naturezaJuridica;
                }
                documento.DescricaoNaturezaJuridica = response.Dados.Atividade.NaturezaJuridicaDescricao;
            }

            // Adiciona sócios
            if (response.Dados.QuadroSocietario != null && response.Dados.QuadroSocietario.Any())
            {
                foreach (var socioDTO in response.Dados.QuadroSocietario)
                {
                    var socio = QuadroSocietario.Criar(
                        documento.Id,
                        socioDTO.CpfCnpj,
                        socioDTO.Nome,
                        socioDTO.Qualificacao);

                    socio.DataEntrada = DateConversionHelper.ParseSerasaDate(socioDTO.DataEntrada);
                    socio.PercentualCapital = DecimalConversionHelper.ConvertToDecimal(socioDTO.PercentualCapital);

                    documento.AdicionarQuadroSocietario(socio);
                }
            }

            // TODO: Adicionar endereços, telefones e emails

            return documento;
        }

        private async Task<Documento> SalvarDocumentoAsync(Documento documento, CancellationToken cancellationToken)
        {
            var documentoDTO = _mapper.Map<DocumentoDTO>(documento);
            var result = await _documentoService.AddAsync(documentoDTO);
            return _mapper.Map<Documento>(result);
        }

        private async Task RegistrarAuditoriaAsync(
            Guid aplicacaoId,
            string numeroDocumento,
            TipoDocumento tipoDocumento,
            bool sucesso,
            string provedoresUtilizados,
            string? provedorPrincipal,
            long tempoProcessamentoMs,
            bool origemCache,
            string? mensagem)
        {
            try
            {
                var aplicacao = await _aplicacaoProvedorRepository.GetByIdAsync(aplicacaoId);
                var nomeAplicacao = aplicacao?.Aplicacao?.Nome ?? "Desconhecida";

                var log = LogAuditoria.Criar(
                    aplicacaoId,
                    nomeAplicacao,
                    numeroDocumento,
                    tipoDocumento,
                    sucesso,
                    tempoProcessamentoMs,
                    DateTime.UtcNow,
                    provedoresUtilizados: provedoresUtilizados,
                    provedorPrincipal: provedorPrincipal,
                    mensagemRetorno: mensagem,
                    origemCache: origemCache);

                await _logAuditoriaRepository.AddAsync(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar auditoria");
            }
        }

        private async Task RegistrarErroAsync(
            Guid aplicacaoId,
            string provedor,
            string erro,
            string? stackTrace)
        {
            try
            {
                var logErro = LogErro.Criar(
                    DateTime.UtcNow,
                    provedor,
                    "ConsultarDocumento",
                    erro,
                    stackTrace,
                    idSistema: aplicacaoId);

                await _logErroRepository.AddAsync(logErro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar log de erro");
            }
        }
    }
}
