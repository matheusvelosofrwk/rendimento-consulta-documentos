using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaDocumentos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class QuadroSocietarioController : ControllerBase
    {
        private readonly IQuadroSocietarioService _service;

        public QuadroSocietarioController(IQuadroSocietarioService service)
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
        public virtual async Task<IActionResult> Create([FromBody] QuadroSocietarioDTO dto, CancellationToken ct = default)
        {
            var result = await _service.AddAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] QuadroSocietarioDTO dto, CancellationToken ct = default)
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
