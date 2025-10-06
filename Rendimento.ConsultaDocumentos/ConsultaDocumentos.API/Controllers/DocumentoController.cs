using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.DTOs.External;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaDocumentos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentoController : ControllerBase
    {
        private readonly IDocumentoService _service;
        private readonly IExternalDocumentConsultaService _externalConsultaService;
        private readonly IProviderHealthCheckService _healthCheckService;

        public DocumentoController(
            IDocumentoService service,
            IExternalDocumentConsultaService externalConsultaService,
            IProviderHealthCheckService healthCheckService)
        {
            _service = service;
            _externalConsultaService = externalConsultaService;
            _healthCheckService = healthCheckService;
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] DocumentoDTO dto, CancellationToken ct = default)
        {
            var result = await _service.AddAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] DocumentoDTO dto, CancellationToken ct = default)
        {
            dto.Id = id;
            var result = await _service.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _service.DeleteAsync(id);
            return Ok(result);
        }

        // Novos endpoints para consultas externas

        /// <summary>
        /// Consulta CPF em provedores externos (SERPRO/SERASA)
        /// </summary>
        /// <param name="cpf">CPF a ser consultado (com ou sem formatação)</param>
        /// <param name="aplicacaoId">ID da aplicação que está realizando a consulta</param>
        /// <param name="forcarNovaConsulta">Se true, ignora cache e força nova consulta</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Dados do documento consultado</returns>
        [HttpGet("consultar/cpf/{cpf}")]
        public async Task<IActionResult> ConsultarCPF(
            [FromRoute] string cpf,
            [FromQuery] Guid aplicacaoId,
            [FromQuery] bool forcarNovaConsulta = false,
            CancellationToken ct = default)
        {
            var request = new ConsultaDocumentoRequest
            {
                NumeroDocumento = cpf,
                TipoDocumento = TipoDocumento.CPF,
                AplicacaoId = aplicacaoId,
                ForcarNovaConsulta = forcarNovaConsulta
            };

            var response = await _externalConsultaService.ConsultarDocumentoAsync(request, ct);

            if (!response.Sucesso)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Consulta CNPJ em provedores externos (SERPRO/SERASA)
        /// </summary>
        /// <param name="cnpj">CNPJ a ser consultado (com ou sem formatação)</param>
        /// <param name="aplicacaoId">ID da aplicação que está realizando a consulta</param>
        /// <param name="perfil">Perfil de consulta (1=Básico, 2=Completo, 3=Com Sócios). Padrão: 3</param>
        /// <param name="forcarNovaConsulta">Se true, ignora cache e força nova consulta</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Dados do documento consultado</returns>
        [HttpGet("consultar/cnpj/{cnpj}")]
        public async Task<IActionResult> ConsultarCNPJ(
            [FromRoute] string cnpj,
            [FromQuery] Guid aplicacaoId,
            [FromQuery] int perfil = 3,
            [FromQuery] bool forcarNovaConsulta = false,
            CancellationToken ct = default)
        {
            if (perfil < 1 || perfil > 3)
            {
                return BadRequest(new { erro = "Perfil deve ser 1, 2 ou 3" });
            }

            var request = new ConsultaDocumentoRequest
            {
                NumeroDocumento = cnpj,
                TipoDocumento = TipoDocumento.CNPJ,
                PerfilCNPJ = perfil,
                AplicacaoId = aplicacaoId,
                ForcarNovaConsulta = forcarNovaConsulta
            };

            var response = await _externalConsultaService.ConsultarDocumentoAsync(request, ct);

            if (!response.Sucesso)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Consulta score de crédito de CPF ou CNPJ (via SERASA)
        /// </summary>
        /// <param name="documento">CPF ou CNPJ a ser consultado</param>
        /// <param name="tipoDocumento">Tipo do documento: CPF ou CNPJ</param>
        /// <param name="aplicacaoId">ID da aplicação que está realizando a consulta</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Score de crédito e análise de risco</returns>
        [HttpGet("consultar/score/{documento}")]
        public async Task<IActionResult> ConsultarScore(
            [FromRoute] string documento,
            [FromQuery] string tipoDocumento,
            [FromQuery] Guid aplicacaoId,
            CancellationToken ct = default)
        {
            if (!Enum.TryParse<TipoDocumento>(tipoDocumento, true, out var tipo))
            {
                return BadRequest(new { erro = "Tipo de documento inválido. Use CPF ou CNPJ" });
            }

            var request = new ConsultaScoreRequest
            {
                NumeroDocumento = documento,
                TipoDocumento = tipo,
                AplicacaoId = aplicacaoId
            };

            var response = await _externalConsultaService.ConsultarScoreAsync(request, ct);

            if (!response.Sucesso)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Verifica o status de saúde de todos os provedores externos
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Status de cada provedor configurado</returns>
        [HttpGet("providers/health")]
        public async Task<IActionResult> VerificarSaudeProvedores(CancellationToken ct = default)
        {
            var response = await _healthCheckService.CheckAllProvidersAsync(ct);
            return Ok(response);
        }

        /// <summary>
        /// Verifica o status de saúde de um provedor específico
        /// </summary>
        /// <param name="providerName">Nome do provedor (SERPRO ou SERASA)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Status do provedor</returns>
        [HttpGet("providers/health/{providerName}")]
        public async Task<IActionResult> VerificarSaudeProvedor(
            [FromRoute] string providerName,
            CancellationToken ct = default)
        {
            var response = await _healthCheckService.CheckProviderAsync(providerName, ct);
            return Ok(response);
        }
    }
}
