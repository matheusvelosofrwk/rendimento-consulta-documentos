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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace ConsultaDocumentos.Application.Services.External
{
    public class ExternalDocumentConsultaService : IExternalDocumentConsultaService
    {
        // Serviços Mock
        private readonly ISerproServiceMock _serproServiceMock;
        private readonly ISerasaServiceMock _serasaServiceMock;

        // Serviços Reais
        private readonly ISerproService? _serproServiceReal;
        private readonly ISerasaService? _serasaServiceReal;

        // Seletor de provedor
        private readonly IProviderSelector _providerSelector;

        private readonly IDocumentoService _documentoService;
        private readonly IAplicacaoProvedorRepository _aplicacaoProvedorRepository;
        private readonly IProvedorRepository _provedorRepository;
        private readonly ILogAuditoriaRepository _logAuditoriaRepository;
        private readonly ILogErroRepository _logErroRepository;
        private readonly ICacheService _cacheService;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<ExternalDocumentConsultaService> _logger;

        public ExternalDocumentConsultaService(
            ISerproServiceMock serproServiceMock,
            ISerasaServiceMock serasaServiceMock,
            IProviderSelector providerSelector,
            IDocumentoService documentoService,
            IAplicacaoProvedorRepository aplicacaoProvedorRepository,
            IProvedorRepository provedorRepository,
            ILogAuditoriaRepository logAuditoriaRepository,
            ILogErroRepository logErroRepository,
            ICacheService cacheService,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ILogger<ExternalDocumentConsultaService> logger,
            ISerproService? serproServiceReal = null,
            ISerasaService? serasaServiceReal = null)
        {
            _serproServiceMock = serproServiceMock;
            _serasaServiceMock = serasaServiceMock;
            _serproServiceReal = serproServiceReal;
            _serasaServiceReal = serasaServiceReal;
            _providerSelector = providerSelector;
            _documentoService = documentoService;
            _aplicacaoProvedorRepository = aplicacaoProvedorRepository;
            _provedorRepository = provedorRepository;
            _logAuditoriaRepository = logAuditoriaRepository;
            _logErroRepository = logErroRepository;
            _cacheService = cacheService;
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _logger = logger;

            _logger.LogInformation("ExternalDocumentConsultaService inicializado no modo: {Modo}",
                _providerSelector.ObterModoAtual());
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

                // Buscar o provedor usado para obter dias de validade
                var provedorUsado = await _provedorRepository.GetByNomeAsync(provedorUtilizado);
                if (provedorUsado == null)
                {
                    throw new InvalidOperationException($"Provedor {provedorUtilizado} não encontrado");
                }

                // Calcular validade dinâmica baseada no tipo de documento
                var diasValidade = request.TipoDocumento == TipoDocumento.CPF
                    ? provedorUsado.QtdDiasValidadePF
                    : provedorUsado.QtdDiasValidadePJ;

                // Atualizar DataConsultaValidade do documento
                documentoConsultado.DataConsultaValidade = DateTime.UtcNow.AddDays(diasValidade);
                await _documentoService.UpdateAsync(_mapper.Map<DocumentoDTO>(documentoConsultado));

                // Atualiza o cache com validade dinâmica
                await _cacheService.SetAsync(
                    cacheKey,
                    documentoDTOResult,
                    TimeSpan.FromDays(diasValidade),
                    cancellationToken);

                _logger.LogInformation(
                    "Documento armazenado no cache com validade de {Dias} dias (provedor: {Provedor})",
                    diasValidade, provedorUtilizado);

                // Registrar log de uso para billing
                await RegistrarLogDeUsoAsync(
                    request.AplicacaoId,
                    provedorUsado.Id,
                    documentoSalvo.Id,
                    cancellationToken);

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

                // Por enquanto, apenas SERASA oferece consulta de score (usando Mock)
                var tipoDocumentoStr = request.TipoDocumento == TipoDocumento.CPF ? "CPF" : "CNPJ";

                var scoreResponse = await _serasaServiceMock.ConsultarScoreAsync(
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
            var cacheKey = $"provedores:aplicacao:{aplicacaoId}";

            // Tentar buscar do cache em memória
            if (_memoryCache.TryGetValue<List<Provedor>>(cacheKey, out var provedoresCached))
            {
                _logger.LogInformation("Provedores retornados do cache em memória para aplicação {AplicacaoId}", aplicacaoId);
                return provedoresCached!;
            }

            _logger.LogInformation("Cache miss: buscando provedores do BD para aplicação {AplicacaoId}", aplicacaoId);

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
                    // CONTROLE DE QUOTA SERPRO
                    if (provedor.Nome.ToUpperInvariant() == "SERPRO" && provedor.QtdAcessoMaximo.HasValue)
                    {
                        // Contar consultas SERPRO hoje
                        var totalConsultasHoje = await ContarConsultasProvedorHoje(provedor.Id);

                        if (totalConsultasHoje >= provedor.QtdAcessoMaximo.Value)
                        {
                            _logger.LogWarning(
                                "Quota SERPRO atingida: {Total}/{Max}. Pulando provedor.",
                                totalConsultasHoje, provedor.QtdAcessoMaximo.Value);
                            continue; // Pula SERPRO
                        }

                        _logger.LogInformation(
                            "Quota SERPRO: {Total}/{Max}",
                            totalConsultasHoje, provedor.QtdAcessoMaximo.Value);
                    }

                    provedores.Add(provedor);
                }
            }

            // Fallback: se não houver provedores configurados, usa SERPRO e SERASA na ordem padrão
            if (!provedores.Any())
            {
                var todosProvedores = await _provedorRepository.GetAllAsync();

                foreach (var provedor in todosProvedores.Where(p => p.Status.ToUpperInvariant() == "ATIVO").OrderBy(p => p.Prioridade))
                {
                    // Aplicar controle de quota também no fallback
                    if (provedor.Nome.ToUpperInvariant() == "SERPRO" && provedor.QtdAcessoMaximo.HasValue)
                    {
                        var totalConsultasHoje = await ContarConsultasProvedorHoje(provedor.Id);
                        if (totalConsultasHoje >= provedor.QtdAcessoMaximo.Value)
                        {
                            continue;
                        }
                    }

                    provedores.Add(provedor);
                }
            }

            // Armazenar no cache por 5 minutos (conforme sistema legado)
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                Priority = CacheItemPriority.Normal
            };

            _memoryCache.Set(cacheKey, provedores, cacheOptions);
            _logger.LogInformation("Provedores armazenados no cache em memória (5 minutos) para aplicação {AplicacaoId}", aplicacaoId);

            return provedores;
        }

        private async Task<int> ContarConsultasProvedorHoje(Guid provedorId)
        {
            var hoje = DateTime.UtcNow.Date;
            var amanha = hoje.AddDays(1);

            // Conta em LogAuditoria
            var logsHoje = await _logAuditoriaRepository.GetAllAsync();
            var totalLogs = logsHoje
                .Where(l => l.ProvedorPrincipal == "SERPRO"
                            && l.DataHoraConsulta >= hoje
                            && l.DataHoraConsulta < amanha)
                .Count();

            return totalLogs;
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
            var usarMock = _providerSelector.UsarMock();

            if (tipoDocumento == TipoDocumento.CPF)
            {
                if (usarMock)
                {
                    // Usar Mock
                    var response = await _serproServiceMock.ConsultarCPFAsync(numeroDocumento, cancellationToken);
                    return ConverterSerprocCPFParaDocumento(response, numeroDocumento);
                }
                else
                {
                    // Usar Real
                    if (_serproServiceReal == null)
                    {
                        throw new InvalidOperationException("Serviço real SERPRO não configurado");
                    }

                    var response = await _serproServiceReal.ConsultarCPFAsync(numeroDocumento, cancellationToken);
                    return ConverterSerprocCPFRealParaDocumento(response, numeroDocumento);
                }
            }
            else
            {
                if (usarMock)
                {
                    // Consulta CNPJ Mock conforme perfil solicitado
                    switch (perfilCNPJ)
                    {
                        case 1:
                            var response1 = await _serproServiceMock.ConsultarCNPJPerfil1Async(numeroDocumento, cancellationToken);
                            return ConverterSerprocCNPJPerfil1ParaDocumento(response1, numeroDocumento);

                        case 2:
                            var response2 = await _serproServiceMock.ConsultarCNPJPerfil2Async(numeroDocumento, cancellationToken);
                            return ConverterSerprocCNPJPerfil2ParaDocumento(response2, numeroDocumento);

                        case 3:
                        default:
                            var response3 = await _serproServiceMock.ConsultarCNPJPerfil3Async(numeroDocumento, cancellationToken);
                            return ConverterSerprocCNPJPerfil3ParaDocumento(response3, numeroDocumento);
                    }
                }
                else
                {
                    // Consulta CNPJ Real - usar sempre Perfil 7 (completo)
                    if (_serproServiceReal == null)
                    {
                        throw new InvalidOperationException("Serviço real SERPRO não configurado");
                    }

                    var response = await _serproServiceReal.ConsultarCNPJPerfil7Async(numeroDocumento, "SISTEMA", cancellationToken);
                    return ConverterSerprocCNPJPerfil7RealParaDocumento(response, numeroDocumento);
                }
            }
        }

        private async Task<Documento?> ConsultarSerasaAsync(
            string numeroDocumento,
            TipoDocumento tipoDocumento,
            CancellationToken cancellationToken)
        {
            var usarMock = _providerSelector.UsarMock();

            if (tipoDocumento == TipoDocumento.CPF)
            {
                if (usarMock)
                {
                    // Usar Mock
                    var response = await _serasaServiceMock.ConsultarCPFAsync(numeroDocumento, "COMPLETA", cancellationToken);
                    return ConverterSerasaCPFParaDocumento(response, numeroDocumento);
                }
                else
                {
                    // Usar Real
                    if (_serasaServiceReal == null)
                    {
                        throw new InvalidOperationException("Serviço real SERASA não configurado");
                    }

                    var response = await _serasaServiceReal.ConsultarCPFAsync(numeroDocumento, 1, cancellationToken);
                    return ConverterSerasaCPFRealParaDocumento(response, numeroDocumento);
                }
            }
            else
            {
                if (usarMock)
                {
                    // Usar Mock
                    var response = await _serasaServiceMock.ConsultarCNPJAsync(numeroDocumento, "COMPLETA", cancellationToken);
                    return ConverterSerasaCNPJParaDocumento(response, numeroDocumento);
                }
                else
                {
                    // Usar Real
                    if (_serasaServiceReal == null)
                    {
                        throw new InvalidOperationException("Serviço real SERASA não configurado");
                    }

                    var response = await _serasaServiceReal.ConsultarCNPJAsync(numeroDocumento, 1, cancellationToken);
                    return ConverterSerasaCNPJRealParaDocumento(response, numeroDocumento);
                }
            }
        }

        // Métodos de conversão (SERPRO Mock -> Documento)

        private Documento ConverterSerprocCPFParaDocumento(SerprocCPFResponseMock response, string numeroDocumento)
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

        private Documento ConverterSerprocCNPJPerfil1ParaDocumento(SerprocCNPJPerfil1ResponseMock response, string numeroDocumento)
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

        private Documento ConverterSerprocCNPJPerfil2ParaDocumento(SerprocCNPJPerfil2ResponseMock response, string numeroDocumento)
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

        private Documento ConverterSerprocCNPJPerfil3ParaDocumento(SerprocCNPJPerfil3ResponseMock response, string numeroDocumento)
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

        private Documento ConverterSerasaCPFParaDocumento(SerasaCPFResponseMock response, string numeroDocumento)
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

        private Documento ConverterSerasaCNPJParaDocumento(SerasaCNPJResponseMock response, string numeroDocumento)
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

        // Métodos de conversão (SERPRO Real -> Documento)

        private Documento ConverterSerprocCPFRealParaDocumento(SerprocCPFResponse response, string numeroDocumento)
        {
            var dadosCPF = response.RetornoConsultada?.FirstOrDefault();
            if (dadosCPF == null || !dadosCPF.DadosValidos)
            {
                throw new ExternalProviderException("SERPRO", "Dados de CPF inválidos ou não encontrados");
            }

            var documento = Documento.CriarPessoaFisica(
                DocumentoValidationHelper.RemoverFormatacao(numeroDocumento),
                dadosCPF.NomeContribuinte ?? string.Empty,
                DateConversionHelper.ParseSerproDateDDMMYYYY(dadosCPF.DataNascimento) ?? DateTime.MinValue);

            documento.NomeMae = dadosCPF.NomeMae;
            documento.Sexo = dadosCPF.CodSexo == "1" ? "M" : dadosCPF.CodSexo == "2" ? "F" : null;
            documento.ResidenteExterior = dadosCPF.IndResidExt == "S";
            documento.AnoObito = DateConversionHelper.ParseAnoObito(dadosCPF.AnoObito);
            documento.IdSituacao = SituacaoCadastralMapper.MapearSituacaoSerpro(dadosCPF.CodSitCad);
            documento.OrigemBureau = "SERPRO";

            return documento;
        }

        private Documento ConverterSerprocCNPJPerfil7RealParaDocumento(SerprocCNPJPerfil7Response response, string numeroDocumento)
        {
            if (response.TemErro)
            {
                throw new ExternalProviderException("SERPRO", response.Erro ?? "Erro desconhecido");
            }

            var documento = Documento.CriarPessoaJuridica(
                DocumentoValidationHelper.RemoverFormatacao(numeroDocumento),
                response.NomeEmpresarial ?? string.Empty,
                DateConversionHelper.ParseSerproDateSlash(response.DataAbertura));

            documento.NomeFantasia = response.NomeFantasia;
            documento.Matriz = response.Estabelecimento?.ToUpperInvariant() == "0001";
            documento.DataSituacao = DateConversionHelper.ParseSerproDateSlash(response.DataSituacaoCadastral);
            documento.IdSituacao = SituacaoCadastralMapper.MapearSituacao(response.SituacaoCadastral);
            documento.OrigemBureau = "SERPRO";

            if (int.TryParse(response.NaturezaJuridica?.Split('-').FirstOrDefault()?.Trim(), out int naturezaJuridica))
            {
                documento.NaturezaJuridica = naturezaJuridica;
            }
            documento.DescricaoNaturezaJuridica = response.NaturezaJuridica;

            // Adicionar sócios do Perfil 7
            if (response.Sociedade != null && response.Sociedade.Any())
            {
                foreach (var socioDTO in response.Sociedade)
                {
                    var socio = QuadroSocietario.Criar(
                        documento.Id,
                        socioDTO.CPFCNPJSocio ?? string.Empty,
                        socioDTO.NomeSocio ?? string.Empty,
                        socioDTO.QualificacaoSocio);

                    socio.DataEntrada = DateConversionHelper.ParseSerproDateSlash(socioDTO.DataEntrada);
                    socio.CpfRepresentanteLegal = socioDTO.CPFRepresentante;
                    socio.NomeRepresentanteLegal = socioDTO.NomeRepresentante;
                    socio.QualificacaoRepresentanteLegal = socioDTO.QualificacaoRepresentante;

                    documento.AdicionarQuadroSocietario(socio);
                }
            }

            return documento;
        }

        // Métodos de conversão (SERASA Real -> Documento)

        private Documento ConverterSerasaCPFRealParaDocumento(SerasaCPFResponse response, string numeroDocumento)
        {
            if (response.TemErro)
            {
                throw new ExternalProviderException("SERASA", response.Erro ?? "Erro desconhecido");
            }

            if (string.IsNullOrWhiteSpace(response.Nome))
            {
                throw new ExternalProviderException("SERASA", "Nome em branco");
            }

            var documento = Documento.CriarPessoaFisica(
                DocumentoValidationHelper.RemoverFormatacao(numeroDocumento),
                response.Nome,
                DateTime.MinValue); // Data de nascimento não vem no retorno básico

            documento.IdSituacao = SituacaoCadastralMapper.MapearSituacao(response.Situacao);
            documento.CodControle = response.Codigo_de_Controle;
            documento.OrigemBureau = "SERASA";

            return documento;
        }

        private Documento ConverterSerasaCNPJRealParaDocumento(SerasaCNPJResponse response, string numeroDocumento)
        {
            if (response.TemErro)
            {
                throw new ExternalProviderException("SERASA", response.Erro ?? "Erro desconhecido");
            }

            if (string.IsNullOrWhiteSpace(response.Nome))
            {
                throw new ExternalProviderException("SERASA", "Nome em branco");
            }

            var documento = Documento.CriarPessoaJuridica(
                DocumentoValidationHelper.RemoverFormatacao(numeroDocumento),
                response.Nome,
                DateConversionHelper.ParseSerasaDate(response.DataAbertura));

            documento.DataSituacao = DateConversionHelper.ParseSerasaDate(response.DataSituacao);
            documento.IdSituacao = SituacaoCadastralMapper.MapearSituacao(response.Situacao);
            documento.OrigemBureau = "SERASA";

            return documento;
        }

        private async Task<Documento> SalvarDocumentoAsync(Documento documento, CancellationToken cancellationToken)
        {
            var documentoDTO = _mapper.Map<DocumentoDTO>(documento);
            var result = await _documentoService.AddAsync(documentoDTO);
            return _mapper.Map<Documento>(result);
        }

        private async Task RegistrarLogDeUsoAsync(
            Guid aplicacaoId,
            Guid provedorId,
            Guid idDocumento,
            CancellationToken cancellationToken)
        {
            try
            {
                // Obter IP e Host da requisição (via HttpContext se disponível)
                string? enderecoIP = null;
                string? remoteHost = null;

                var httpContext = _httpContextAccessor?.HttpContext;
                if (httpContext != null)
                {
                    enderecoIP = httpContext.Connection.RemoteIpAddress?.ToString();
                    remoteHost = httpContext.Request.Host.Value;
                }

                var logDeUso = AplicacaoProvedor.CriarLogDeUso(
                    aplicacaoId,
                    provedorId,
                    idDocumento,
                    enderecoIP,
                    remoteHost);

                await _aplicacaoProvedorRepository.AddAsync(logDeUso);

                _logger.LogInformation(
                    "Log de uso registrado: Aplicação {AplicacaoId}, Provedor {ProvedorId}, Documento {DocumentoId}",
                    aplicacaoId, provedorId, idDocumento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar log de uso");
                // Não propagar exceção - log de uso é informativo
            }
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
