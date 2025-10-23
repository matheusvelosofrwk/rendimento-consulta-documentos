using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaDocumentos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AplicacaoProvedorController : ControllerBase
    {
        private readonly IAplicacaoProvedorService _service;

        public AplicacaoProvedorController(IAplicacaoProvedorService service)
        {
            _service = service;
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
        public virtual async Task<IActionResult> Create([FromBody] AplicacaoProvedorDTO dto, CancellationToken ct = default)
        {
            var result = await _service.AddAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] AplicacaoProvedorDTO dto, CancellationToken ct = default)
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

        [HttpGet("ByAplicacao/{aplicacaoId}")]
        public virtual async Task<IActionResult> GetByAplicacaoId([FromRoute] Guid aplicacaoId, CancellationToken ct = default)
        {
            var result = await _service.GetByAplicacaoIdAsync(aplicacaoId);
            return Ok(result);
        }
    }
}
