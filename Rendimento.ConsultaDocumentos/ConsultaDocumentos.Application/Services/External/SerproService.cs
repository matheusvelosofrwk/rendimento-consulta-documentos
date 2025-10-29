using ConsultaDocumentos.Application.DTOs.External.Serpro;
using ConsultaDocumentos.Application.Exceptions;
using ConsultaDocumentos.Application.Helpers;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ConsultaDocumentos.Application.Services.External
{
    public class SerproService : ISerproService
    {
        private readonly IConfiguration _configuration;
        private readonly ICertificadoManager _certificadoManager;
        private readonly ILogger<SerproService> _logger;

        private const string SERPRO_NOME = "SERPRO";
        private const string CPF_USUARIO = "25008464825"; // Constante do sistema
        private const string NAMESPACE_URI = "https://acesso.infoconv.receita.fazenda.gov.br/ws/cnpj/";

        public SerproService(
            IConfiguration configuration,
            ICertificadoManager certificadoManager,
            ILogger<SerproService> logger)
        {
            _configuration = configuration;
            _certificadoManager = certificadoManager;
            _logger = logger;
        }

        /// <summary>
        /// Consulta CPF utilizando o protocolo REST/JSON do Serpro
        /// </summary>
        /// <remarks>
        /// PROTOCOLO: REST
        /// - Content-Type: application/json
        /// - Método: POST (HttpWebRequest)
        /// - Request: JSON (SerprocCPFRequest)
        /// - Response: JSON (SerprocCPFResponse)
        /// - Autenticação: Certificado Digital (.pfx)
        /// </remarks>
        public async Task<SerprocCPFResponse> ConsultarCPFAsync(
            string cpf,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Iniciando consulta CPF no Serpro (REST): {CPF}", cpf);

                // Montar request JSON
                var request = new SerprocCPFRequest
                {
                    Listadecpf = cpf,
                    CpfUsuario = CPF_USUARIO
                };

                var jsonRequest = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var jsonBytes = Encoding.UTF8.GetBytes(jsonRequest);

                // Obter URL
                var cpfUrl = _configuration["ExternalProviders:Serpro:Real:CpfUrl"];
                if (string.IsNullOrEmpty(cpfUrl))
                {
                    throw new InvalidOperationException("URL CPF Serpro não configurada");
                }

                _logger.LogDebug("Enviando requisição REST para {Url}", cpfUrl);

                // Criar HttpWebRequest com certificado
                var httpRequest = (HttpWebRequest)WebRequest.Create(cpfUrl);
                httpRequest.ContentType = "application/json";
                httpRequest.Method = "POST";
                httpRequest.ContentLength = jsonBytes.Length;

                // Adicionar certificado digital
                var certificado = await _certificadoManager.ObterCertificadoAsync(cancellationToken);
                httpRequest.ClientCertificates.Add(certificado);

                // Enviar request
                using (var stream = httpRequest.GetRequestStream())
                {
                    await stream.WriteAsync(jsonBytes, 0, jsonBytes.Length, cancellationToken);
                }

                // Ler response
                string responseJson;
                using (var httpResponse = (HttpWebResponse)await httpRequest.GetResponseAsync())
                using (var stream = httpResponse.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    responseJson = await reader.ReadToEndAsync();
                }

                _logger.LogDebug("Resposta REST recebida em {Tempo}ms", stopwatch.ElapsedMilliseconds);

                // Deserializar response
                var response = JsonSerializer.Deserialize<SerprocCPFResponse>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (response == null)
                {
                    throw new ExternalProviderException(SERPRO_NOME, "Falha ao deserializar resposta");
                }

                // Validar codRetorno
                if (response.TemErro)
                {
                    _logger.LogWarning("Erro retornado pelo Serpro: {Erro}", response.MsgRetorno);
                    throw new ExternalProviderException(SERPRO_NOME, response.MsgRetorno);
                }

                // Validar dados do primeiro registro
                var dadosCPF = response.RetornoConsultada?.FirstOrDefault();
                if (dadosCPF == null || !dadosCPF.DadosValidos)
                {
                    var mensagem = dadosCPF?.MsgRetorno ?? "CPF não encontrado";
                    _logger.LogWarning("CPF não encontrado no Serpro: {Mensagem}", mensagem);
                    throw new ExternalProviderException(SERPRO_NOME, mensagem);
                }

                _logger.LogInformation("Consulta CPF Serpro concluída com sucesso em {Tempo}ms", stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (ExternalProviderException)
            {
                throw;
            }
            catch (WebException ex)
            {
                _logger.LogError(ex, "Erro de comunicação com Serpro");
                throw new ExternalProviderException(SERPRO_NOME, "Falha na comunicação com o serviço", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao consultar CPF no Serpro");
                throw new ExternalProviderException(SERPRO_NOME, "Erro inesperado", ex);
            }
        }

        public async Task<SerprocCNPJPerfil1Response> ConsultarCNPJPerfil1Async(
            string cnpj,
            string sistemaConvenente,
            CancellationToken cancellationToken = default)
        {
            return await ConsultarCNPJAsync<SerprocCNPJPerfil1Response>(cnpj, 1, sistemaConvenente, cancellationToken);
        }

        public async Task<SerprocCNPJPerfil2Response> ConsultarCNPJPerfil2Async(
            string cnpj,
            string sistemaConvenente,
            CancellationToken cancellationToken = default)
        {
            return await ConsultarCNPJAsync<SerprocCNPJPerfil2Response>(cnpj, 2, sistemaConvenente, cancellationToken);
        }

        public async Task<SerprocCNPJPerfil3Response> ConsultarCNPJPerfil3Async(
            string cnpj,
            string sistemaConvenente,
            CancellationToken cancellationToken = default)
        {
            return await ConsultarCNPJAsync<SerprocCNPJPerfil3Response>(cnpj, 3, sistemaConvenente, cancellationToken);
        }

        public async Task<SerprocCNPJPerfil7Response> ConsultarCNPJPerfil7Async(
            string cnpj,
            string sistemaConvenente,
            CancellationToken cancellationToken = default)
        {
            return await ConsultarCNPJAsync<SerprocCNPJPerfil7Response>(cnpj, 7, sistemaConvenente, cancellationToken);
        }

        /// <summary>
        /// Consulta CNPJ utilizando o protocolo SOAP/XML do Serpro
        /// </summary>
        /// <remarks>
        /// PROTOCOLO: SOAP
        /// - Content-Type: text/xml; charset=utf-8
        /// - Método: POST (HttpWebRequest)
        /// - Request: XML SOAP Envelope (gerado via SoapHelper)
        /// - Response: XML SOAP (parseado via XmlDocument)
        /// - Autenticação: Certificado Digital (.pfx)
        /// - SOAPAction Header: Obrigatório (namespace + operação)
        /// - Perfis Disponíveis: 1 (Básico), 2 (Completo), 3 (Com Sócios), 7 (Detalhado)
        /// </remarks>
        private async Task<T> ConsultarCNPJAsync<T>(
            string cnpj,
            int perfil,
            string sistemaConvenente,
            CancellationToken cancellationToken) where T : SerprocCNPJPerfil1Response, new()
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Iniciando consulta CNPJ no Serpro (SOAP) Perfil {Perfil}: {CNPJ}", perfil, cnpj);

                // Montar requisição SOAP
                var operacao = $"ConsultarCNPJP{perfil}_SC";
                var parametros = new Dictionary<string, string>
                {
                    { "CNPJ", cnpj },
                    { "CPFUsuario", CPF_USUARIO },
                    { "SistemaConvenente", sistemaConvenente }
                };

                var soapEnvelope = SoapHelper.CriarEnvelopeSerpro(operacao, parametros, NAMESPACE_URI);

                // Obter URL
                var cnpjUrl = _configuration["ExternalProviders:Serpro:Real:CnpjUrl"];
                if (string.IsNullOrEmpty(cnpjUrl))
                {
                    throw new InvalidOperationException("URL CNPJ Serpro não configurada");
                }

                _logger.LogDebug("Enviando requisição SOAP para {Url}", cnpjUrl);

                // Criar HttpWebRequest com certificado
                var soapBytes = Encoding.UTF8.GetBytes(soapEnvelope);
                var httpRequest = (HttpWebRequest)WebRequest.Create(cnpjUrl);
                httpRequest.ContentType = "text/xml; charset=utf-8";
                httpRequest.Method = "POST";
                httpRequest.ContentLength = soapBytes.Length;
                httpRequest.Headers.Add("SOAPAction", $"{NAMESPACE_URI}{operacao}");

                // Adicionar certificado digital
                var certificado = await _certificadoManager.ObterCertificadoAsync(cancellationToken);
                httpRequest.ClientCertificates.Add(certificado);

                // Enviar request
                using (var stream = httpRequest.GetRequestStream())
                {
                    await stream.WriteAsync(soapBytes, 0, soapBytes.Length, cancellationToken);
                }

                // Ler response
                string responseXml;
                using (var httpResponse = (HttpWebResponse)await httpRequest.GetResponseAsync())
                using (var stream = httpResponse.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    responseXml = await reader.ReadToEndAsync();
                }

                _logger.LogDebug("Resposta SOAP recebida em {Tempo}ms", stopwatch.ElapsedMilliseconds);

                // Parsear resposta XML
                var doc = SoapHelper.ParsearRespostaSoap(responseXml);

                // Extrair dados conforme perfil
                var response = ExtrairDadosCNPJ<T>(doc, perfil);

                // Validar erro
                if (response.TemErro)
                {
                    _logger.LogWarning("Erro retornado pelo Serpro: {Erro}", response.Erro);
                    throw new ExternalProviderException(SERPRO_NOME, response.Erro ?? "Erro desconhecido");
                }

                _logger.LogInformation("Consulta CNPJ Serpro Perfil {Perfil} concluída com sucesso em {Tempo}ms", perfil, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (ExternalProviderException)
            {
                throw;
            }
            catch (WebException ex)
            {
                _logger.LogError(ex, "Erro de comunicação com Serpro");
                throw new ExternalProviderException(SERPRO_NOME, "Falha na comunicação com o serviço", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao consultar CNPJ no Serpro");
                throw new ExternalProviderException(SERPRO_NOME, "Erro inesperado", ex);
            }
        }

        private T ExtrairDadosCNPJ<T>(System.Xml.XmlDocument doc, int perfil) where T : SerprocCNPJPerfil1Response, new()
        {
            var response = new T
            {
                CNPJ = SoapHelper.ExtrairValorXml(doc, "CNPJ"),
                Estabelecimento = SoapHelper.ExtrairValorXml(doc, "Estabelecimento"),
                NomeEmpresarial = SoapHelper.ExtrairValorXml(doc, "NomeEmpresarial"),
                NomeFantasia = SoapHelper.ExtrairValorXml(doc, "NomeFantasia"),
                SituacaoCadastral = SoapHelper.ExtrairValorXml(doc, "SituacaoCadastral"),
                MotivoSituacao = SoapHelper.ExtrairValorXml(doc, "MotivoSituacao"),
                DataSituacaoCadastral = SoapHelper.ExtrairValorXml(doc, "DataSituacaoCadastral"),
                Erro = SoapHelper.ExtrairErroSoap(doc)
            };

            // Perfil 2 ou superior
            if (perfil >= 2 && response is SerprocCNPJPerfil2Response perfil2)
            {
                perfil2.DataAbertura = SoapHelper.ExtrairValorXml(doc, "DataAbertura");
                perfil2.NaturezaJuridica = SoapHelper.ExtrairValorXml(doc, "NaturezaJuridica");
                perfil2.TipoLogradouro = SoapHelper.ExtrairValorXml(doc, "TipoLogradouro");
                perfil2.Logradouro = SoapHelper.ExtrairValorXml(doc, "Logradouro");
                perfil2.NumeroLogradouro = SoapHelper.ExtrairValorXml(doc, "NumeroLogradouro");
                perfil2.Complemento = SoapHelper.ExtrairValorXml(doc, "Complemento");
                perfil2.Bairro = SoapHelper.ExtrairValorXml(doc, "Bairro");
                perfil2.CEP = SoapHelper.ExtrairValorXml(doc, "CEP");
                perfil2.UF = SoapHelper.ExtrairValorXml(doc, "UF");
                perfil2.CodigoMunicipio = SoapHelper.ExtrairValorXml(doc, "CodigoMunicipio");
                perfil2.NomeMunicipio = SoapHelper.ExtrairValorXml(doc, "NomeMunicipio");
                perfil2.Referencia = SoapHelper.ExtrairValorXml(doc, "Referencia");
            }

            // Perfil 3 ou superior
            if (perfil >= 3 && response is SerprocCNPJPerfil3Response perfil3)
            {
                perfil3.CNAEPrincipal = SoapHelper.ExtrairValorXml(doc, "CNAEPrincipal");
                perfil3.DDD1 = SoapHelper.ExtrairValorXml(doc, "DDD1");
                perfil3.Telefone1 = SoapHelper.ExtrairValorXml(doc, "Telefone1");
                perfil3.DDD2 = SoapHelper.ExtrairValorXml(doc, "DDD2");
                perfil3.Telefone2 = SoapHelper.ExtrairValorXml(doc, "Telefone2");
                perfil3.Email = SoapHelper.ExtrairValorXml(doc, "Email");

                // CNAEs secundários
                var cnaesSecundarios = SoapHelper.ObterNosXml(doc, "CNAESecundario");
                if (cnaesSecundarios != null && cnaesSecundarios.Count > 0)
                {
                    perfil3.CNAESecundario = new List<string>();
                    foreach (System.Xml.XmlNode node in cnaesSecundarios)
                    {
                        var valor = node.InnerText?.Trim();
                        if (!string.IsNullOrEmpty(valor))
                        {
                            perfil3.CNAESecundario.Add(valor);
                        }
                    }
                }
            }

            // Perfil 7
            if (perfil == 7 && response is SerprocCNPJPerfil7Response perfil7)
            {
                perfil7.CPFResponsavel = SoapHelper.ExtrairValorXml(doc, "CPFResponsavel");
                perfil7.NomeResponsavel = SoapHelper.ExtrairValorXml(doc, "NomeResponsavel");
                perfil7.CapitalSocial = SoapHelper.ExtrairValorXml(doc, "CapitalSocial");
                perfil7.Porte = SoapHelper.ExtrairValorXml(doc, "Porte");
                perfil7.OpcaoSimples = SoapHelper.ExtrairValorXml(doc, "OpcaoSimples");
                perfil7.OpcaoSIMEI = SoapHelper.ExtrairValorXml(doc, "OpcaoSIMEI");
                perfil7.SituacaoEspecial = SoapHelper.ExtrairValorXml(doc, "SituacaoEspecial");
                perfil7.DataSituacaoEspecial = SoapHelper.ExtrairValorXml(doc, "DataSituacaoEspecial");
                perfil7.CidadeExterior = SoapHelper.ExtrairValorXml(doc, "CidadeExterior");
                perfil7.CodigoPais = SoapHelper.ExtrairValorXml(doc, "CodigoPais");
                perfil7.NomePais = SoapHelper.ExtrairValorXml(doc, "NomePais");

                // Quadro societário
                var sociosNodes = SoapHelper.ObterNosXml(doc, "SocioPerfil7");
                if (sociosNodes != null && sociosNodes.Count > 0)
                {
                    perfil7.Sociedade = new List<SerprocSocioDTO>();
                    foreach (System.Xml.XmlNode socioNode in sociosNodes)
                    {
                        var socio = new SerprocSocioDTO();
                        foreach (System.Xml.XmlNode campo in socioNode.ChildNodes)
                        {
                            var valor = campo.InnerText?.Trim();
                            switch (campo.Name)
                            {
                                case "CPFCNPJSocio":
                                    socio.CPFCNPJSocio = valor;
                                    break;
                                case "NomeSocio":
                                    socio.NomeSocio = valor;
                                    break;
                                case "QualificacaoSocio":
                                    socio.QualificacaoSocio = valor;
                                    break;
                                case "DataEntrada":
                                    socio.DataEntrada = valor;
                                    break;
                                case "CPFRepresentante":
                                    socio.CPFRepresentante = valor;
                                    break;
                                case "NomeRepresentante":
                                    socio.NomeRepresentante = valor;
                                    break;
                                case "QualificacaoRepresentante":
                                    socio.QualificacaoRepresentante = valor;
                                    break;
                            }
                        }
                        perfil7.Sociedade.Add(socio);
                    }
                }
            }

            return response;
        }

        public async Task<SerproHealthCheckResponse> VerificarDisponibilidadeAsync(
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new SerproHealthCheckResponse
            {
                Timestamp = DateTime.UtcNow
            };

            try
            {
                // Verificar se o certificado está disponível
                var certificado = await _certificadoManager.ObterCertificadoAsync(cancellationToken);
                if (certificado == null)
                {
                    response.Status = "ERROR";
                    response.Mensagem = "Certificado não carregado";
                    return response;
                }

                // Verificar URL
                var cnpjUrl = _configuration["ExternalProviders:Serpro:Real:CnpjUrl"];
                if (string.IsNullOrEmpty(cnpjUrl))
                {
                    response.Status = "ERROR";
                    response.Mensagem = "URL não configurada";
                    return response;
                }

                // Tentar acessar o WSDL
                var wsdlUrl = _configuration["ExternalProviders:Serpro:Real:CnpjWsdl"];
                if (!string.IsNullOrEmpty(wsdlUrl))
                {
                    var httpRequest = (HttpWebRequest)WebRequest.Create(wsdlUrl);
                    httpRequest.Method = "GET";
                    httpRequest.ClientCertificates.Add(certificado);

                    using (var httpResponse = (HttpWebResponse)await httpRequest.GetResponseAsync())
                    {
                        stopwatch.Stop();

                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            response.Status = "OK";
                            response.Mensagem = "Serviço disponível";
                            response.TempoRespostaMs = stopwatch.ElapsedMilliseconds;
                        }
                        else
                        {
                            response.Status = "ERROR";
                            response.Mensagem = $"HTTP {(int)httpResponse.StatusCode}";
                            response.TempoRespostaMs = stopwatch.ElapsedMilliseconds;
                        }
                    }
                }
                else
                {
                    response.Status = "UNKNOWN";
                    response.Mensagem = "WSDL URL não configurada";
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                response.Status = "ERROR";
                response.Mensagem = ex.Message;
                response.TempoRespostaMs = stopwatch.ElapsedMilliseconds;
                _logger.LogWarning(ex, "Serpro indisponível");
            }

            return response;
        }
    }
}
