using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Domain.Entities;
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
            var logs = await _repository.GetAllAsync();
            var logsDto = _mapper.Map<IList<LogAuditoriaDTO>>(logs);
            return Ok(logsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var log = await _repository.GetByIdAsync(id);

            if (log == null)
                return NotFound();

            var logDto = _mapper.Map<LogAuditoriaDTO>(log);
            return Ok(logDto);
        }

        [HttpGet("aplicacao/{aplicacaoId}")]
        public async Task<IActionResult> GetByAplicacao([FromRoute] Guid aplicacaoId)
        {
            var logs = await _repository.GetByAplicacaoAsync(aplicacaoId);
            var logsDto = _mapper.Map<IList<LogAuditoriaDTO>>(logs);
            return Ok(logsDto);
        }

        [HttpGet("documento/{documentoNumero}")]
        public async Task<IActionResult> GetByDocumentoNumero([FromRoute] string documentoNumero)
        {
            var logs = await _repository.GetByDocumentoNumeroAsync(documentoNumero);
            var logsDto = _mapper.Map<IList<LogAuditoriaDTO>>(logs);
            return Ok(logsDto);
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetByData([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            var logs = await _repository.GetByDataAsync(dataInicio, dataFim);
            var logsDto = _mapper.Map<IList<LogAuditoriaDTO>>(logs);
            return Ok(logsDto);
        }

        [HttpGet("sucesso/{consultaSucesso}")]
        public async Task<IActionResult> GetByConsultaSucesso([FromRoute] bool consultaSucesso)
        {
            var logs = await _repository.GetByConsultaSucessoAsync(consultaSucesso);
            var logsDto = _mapper.Map<IList<LogAuditoriaDTO>>(logs);
            return Ok(logsDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LogAuditoriaDTO dto)
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
            return CreatedAtAction(nameof(GetById), new { id = log.Id }, logDto);
        }
    }
}
