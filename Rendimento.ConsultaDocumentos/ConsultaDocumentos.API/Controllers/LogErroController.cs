using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaDocumentos.API.Controllers
{
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
            var logs = await _repository.GetAllAsync();
            var logsDto = _mapper.Map<IList<LogErroDTO>>(logs);
            return Ok(logsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var log = await _repository.GetByIdAsync(id);

            if (log == null)
                return NotFound();

            var logDto = _mapper.Map<LogErroDTO>(log);
            return Ok(logDto);
        }

        [HttpGet("aplicacao/{aplicacao}")]
        public async Task<IActionResult> GetByAplicacao([FromRoute] string aplicacao)
        {
            var logs = await _repository.GetByAplicacaoAsync(aplicacao);
            var logsDto = _mapper.Map<IList<LogErroDTO>>(logs);
            return Ok(logsDto);
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetByData([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            var logs = await _repository.GetByDataAsync(dataInicio, dataFim);
            var logsDto = _mapper.Map<IList<LogErroDTO>>(logs);
            return Ok(logsDto);
        }

        [HttpGet("usuario/{usuario}")]
        public async Task<IActionResult> GetByUsuario([FromRoute] string usuario)
        {
            var logs = await _repository.GetByUsuarioAsync(usuario);
            var logsDto = _mapper.Map<IList<LogErroDTO>>(logs);
            return Ok(logsDto);
        }

        [HttpGet("sistema/{idSistema}")]
        public async Task<IActionResult> GetBySistema([FromRoute] Guid idSistema)
        {
            var logs = await _repository.GetBySistemaAsync(idSistema);
            var logsDto = _mapper.Map<IList<LogErroDTO>>(logs);
            return Ok(logsDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LogErroDTO dto)
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
            return CreatedAtAction(nameof(GetById), new { id = log.Id }, logDto);
        }
    }
}
