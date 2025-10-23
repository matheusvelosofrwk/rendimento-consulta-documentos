using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Results;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Enums;
using ConsultaDocumentos.Domain.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaDocumentos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogAuditoriaController : ControllerBase
    {
        private readonly ILogAuditoriaRepository _repository;
        private readonly IMapper _mapper;

        public LogAuditoriaController(ILogAuditoriaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var logs = await _repository.GetAllAsync();
                var logsDto = _mapper.Map<IList<LogAuditoriaDTO>>(logs);
                return Ok(Result<IList<LogAuditoriaDTO>>.SuccessResult(logsDto));
            }
            catch (Exception ex)
            {
                return Ok(Result<IList<LogAuditoriaDTO>>.FailureResult(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                var log = await _repository.GetByIdAsync(id);

                if (log == null)
                    return Ok(Result<LogAuditoriaDTO>.FailureResult("Registro não encontrado"));

                var logDto = _mapper.Map<LogAuditoriaDTO>(log);
                return Ok(Result<LogAuditoriaDTO>.SuccessResult(logDto));
            }
            catch (Exception ex)
            {
                return Ok(Result<LogAuditoriaDTO>.FailureResult(ex.Message));
            }
        }

        [HttpGet("aplicacao/{aplicacaoId}")]
        public async Task<IActionResult> GetByAplicacao([FromRoute] Guid aplicacaoId)
        {
            try
            {
                var logs = await _repository.GetByAplicacaoAsync(aplicacaoId);
                var logsDto = _mapper.Map<IList<LogAuditoriaDTO>>(logs);
                return Ok(Result<IList<LogAuditoriaDTO>>.SuccessResult(logsDto));
            }
            catch (Exception ex)
            {
                return Ok(Result<IList<LogAuditoriaDTO>>.FailureResult(ex.Message));
            }
        }

        [HttpGet("documento/{documentoNumero}")]
        public async Task<IActionResult> GetByDocumentoNumero([FromRoute] string documentoNumero)
        {
            try
            {
                var logs = await _repository.GetByDocumentoNumeroAsync(documentoNumero);
                var logsDto = _mapper.Map<IList<LogAuditoriaDTO>>(logs);
                return Ok(Result<IList<LogAuditoriaDTO>>.SuccessResult(logsDto));
            }
            catch (Exception ex)
            {
                return Ok(Result<IList<LogAuditoriaDTO>>.FailureResult(ex.Message));
            }
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetByData([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            try
            {
                var logs = await _repository.GetByDataAsync(dataInicio, dataFim);
                var logsDto = _mapper.Map<IList<LogAuditoriaDTO>>(logs);
                return Ok(Result<IList<LogAuditoriaDTO>>.SuccessResult(logsDto));
            }
            catch (Exception ex)
            {
                return Ok(Result<IList<LogAuditoriaDTO>>.FailureResult(ex.Message));
            }
        }

        [HttpGet("sucesso/{consultaSucesso}")]
        public async Task<IActionResult> GetByConsultaSucesso([FromRoute] bool consultaSucesso)
        {
            try
            {
                var logs = await _repository.GetByConsultaSucessoAsync(consultaSucesso);
                var logsDto = _mapper.Map<IList<LogAuditoriaDTO>>(logs);
                return Ok(Result<IList<LogAuditoriaDTO>>.SuccessResult(logsDto));
            }
            catch (Exception ex)
            {
                return Ok(Result<IList<LogAuditoriaDTO>>.FailureResult(ex.Message));
            }
        }

        [HttpGet("filtrar")]
        public async Task<IActionResult> GetWithFilters(
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null,
            [FromQuery] string? numeroDocumento = null,
            [FromQuery] Guid? aplicacaoProvedorId = null,
            [FromQuery] TipoDocumento? tipoDocumento = null)
        {
            try
            {
                var logs = await _repository.GetWithFiltersAsync(
                    dataInicio,
                    dataFim,
                    numeroDocumento,
                    aplicacaoProvedorId,
                    tipoDocumento);

                var logsDto = _mapper.Map<IList<LogAuditoriaDTO>>(logs);
                return Ok(Result<IList<LogAuditoriaDTO>>.SuccessResult(logsDto));
            }
            catch (Exception ex)
            {
                return Ok(Result<IList<LogAuditoriaDTO>>.FailureResult(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LogAuditoriaDTO dto)
        {
            try
            {
                var log = LogAuditoria.Criar(
                    dto.AplicacaoId,
                    dto.NomeAplicacao,
                    dto.DocumentoNumero,
                    dto.TipoDocumento,
                    dto.ConsultaSucesso,
                    dto.TempoProcessamentoMs,
                    dto.DataHoraConsulta,
                    dto.ParametrosEntrada,
                    dto.ProvedoresUtilizados,
                    dto.ProvedorPrincipal,
                    dto.RespostaProvedor,
                    dto.MensagemRetorno,
                    dto.EnderecoIp,
                    dto.UserAgent,
                    dto.TokenAutenticacao,
                    dto.OrigemCache,
                    dto.InformacoesAdicionais
                );

                await _repository.AddAsync(log);

                var logDto = _mapper.Map<LogAuditoriaDTO>(log);
                var result = Result<LogAuditoriaDTO>.SuccessResult(logDto);
                return CreatedAtAction(nameof(GetById), new { id = log.Id }, result);
            }
            catch (Exception ex)
            {
                return Ok(Result<LogAuditoriaDTO>.FailureResult(ex.Message));
            }
        }
    }
}
