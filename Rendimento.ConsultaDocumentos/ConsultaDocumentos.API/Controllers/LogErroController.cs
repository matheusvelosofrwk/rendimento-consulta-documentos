using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Results;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Enums;
using ConsultaDocumentos.Domain.Intefaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaDocumentos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LogErroController : ControllerBase
    {
        private readonly ILogErroRepository _repository;
        private readonly IMapper _mapper;

        public LogErroController(ILogErroRepository repository, IMapper mapper)
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
                var logsDto = _mapper.Map<IList<LogErroDTO>>(logs);
                return Ok(Result<IList<LogErroDTO>>.SuccessResult(logsDto));
            }
            catch (Exception ex)
            {
                return Ok(Result<IList<LogErroDTO>>.FailureResult(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                var log = await _repository.GetByIdAsync(id);

                if (log == null)
                    return Ok(Result<LogErroDTO>.FailureResult("Registro não encontrado"));

                var logDto = _mapper.Map<LogErroDTO>(log);
                return Ok(Result<LogErroDTO>.SuccessResult(logDto));
            }
            catch (Exception ex)
            {
                return Ok(Result<LogErroDTO>.FailureResult(ex.Message));
            }
        }

        [HttpGet("aplicacao/{aplicacao}")]
        public async Task<IActionResult> GetByAplicacao([FromRoute] string aplicacao)
        {
            try
            {
                var logs = await _repository.GetByAplicacaoAsync(aplicacao);
                var logsDto = _mapper.Map<IList<LogErroDTO>>(logs);
                return Ok(Result<IList<LogErroDTO>>.SuccessResult(logsDto));
            }
            catch (Exception ex)
            {
                return Ok(Result<IList<LogErroDTO>>.FailureResult(ex.Message));
            }
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetByData([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            try
            {
                var logs = await _repository.GetByDataAsync(dataInicio, dataFim);
                var logsDto = _mapper.Map<IList<LogErroDTO>>(logs);
                return Ok(Result<IList<LogErroDTO>>.SuccessResult(logsDto));
            }
            catch (Exception ex)
            {
                return Ok(Result<IList<LogErroDTO>>.FailureResult(ex.Message));
            }
        }

        [HttpGet("usuario/{usuario}")]
        public async Task<IActionResult> GetByUsuario([FromRoute] string usuario)
        {
            try
            {
                var logs = await _repository.GetByUsuarioAsync(usuario);
                var logsDto = _mapper.Map<IList<LogErroDTO>>(logs);
                return Ok(Result<IList<LogErroDTO>>.SuccessResult(logsDto));
            }
            catch (Exception ex)
            {
                return Ok(Result<IList<LogErroDTO>>.FailureResult(ex.Message));
            }
        }

        [HttpGet("sistema/{idSistema}")]
        public async Task<IActionResult> GetBySistema([FromRoute] Guid idSistema)
        {
            try
            {
                var logs = await _repository.GetBySistemaAsync(idSistema);
                var logsDto = _mapper.Map<IList<LogErroDTO>>(logs);
                return Ok(Result<IList<LogErroDTO>>.SuccessResult(logsDto));
            }
            catch (Exception ex)
            {
                return Ok(Result<IList<LogErroDTO>>.FailureResult(ex.Message));
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

                var logsDto = _mapper.Map<IList<LogErroDTO>>(logs);
                return Ok(Result<IList<LogErroDTO>>.SuccessResult(logsDto));
            }
            catch (Exception ex)
            {
                return Ok(Result<IList<LogErroDTO>>.FailureResult(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LogErroDTO dto)
        {
            try
            {
                var log = LogErro.Criar(
                    dto.DataHora,
                    dto.Aplicacao,
                    dto.Metodo,
                    dto.Erro,
                    dto.StackTrace,
                    dto.Usuario,
                    dto.IdSistema
                );

                await _repository.AddAsync(log);

                var logDto = _mapper.Map<LogErroDTO>(log);
                var result = Result<LogErroDTO>.SuccessResult(logDto);
                return CreatedAtAction(nameof(GetById), new { id = log.Id }, result);
            }
            catch (Exception ex)
            {
                return Ok(Result<LogErroDTO>.FailureResult(ex.Message));
            }
        }
    }
}
