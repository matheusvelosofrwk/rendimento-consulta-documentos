using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaDocumentos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvedorController : ControllerBase
    {
        private readonly IProvedorService _service;

        public ProvedorController(IProvedorService service)
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
        public virtual async Task<IActionResult> Create([FromBody] ProvedorDTO dto, CancellationToken ct = default)
        {
            // Validações
            if (dto.QtdDiasValidadePF <= 0 || dto.QtdDiasValidadePJ <= 0 || dto.QtdDiasValidadeEND <= 0)
            {
                return BadRequest(new { erro = "Dias de validade devem ser maiores que zero" });
            }

            if (dto.TipoWebService < 1 || dto.TipoWebService > 3)
            {
                return BadRequest(new { erro = "TipoWebService deve ser 1 (CPF), 2 (CNPJ) ou 3 (Ambos)" });
            }

            if (dto.QtdAcessoMaximo.HasValue && dto.QtdAcessoMaximo.Value < 0)
            {
                return BadRequest(new { erro = "QtdAcessoMaximo não pode ser negativo" });
            }

            var result = await _service.AddAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ProvedorDTO dto, CancellationToken ct = default)
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
