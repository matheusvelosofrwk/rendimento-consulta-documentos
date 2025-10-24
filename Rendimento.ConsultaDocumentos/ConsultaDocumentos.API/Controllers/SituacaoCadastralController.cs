using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaDocumentos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SituacaoCadastralController : ControllerBase
    {
        private readonly ISituacaoCadastralService _service;

        public SituacaoCadastralController(ISituacaoCadastralService service)
        {
            _service = service;
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("tipo/{tipoPessoa}")]
        public virtual async Task<IActionResult> GetByTipoPessoa([FromRoute] char tipoPessoa, CancellationToken ct = default)
        {
            var result = await _service.GetByTipoPessoaAsync(tipoPessoa);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] SituacaoCadastralDTO dto, CancellationToken ct = default)
        {
            var result = await _service.AddAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] SituacaoCadastralDTO dto, CancellationToken ct = default)
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
    }
}
