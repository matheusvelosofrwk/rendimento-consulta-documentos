using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaDocumentos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseCRUDController<ViewModel, TEntity> : ControllerBase where TEntity : BaseEntity
    {


        public BaseCRUDController(IBaseService<TEntity> service)
        {

        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            return NoContent();
        }

        //[HttpGet("{id}")]
        //public virtual async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct = default)
        //{

        //}

        //[HttpPost]
        //public virtual async Task<IActionResult> Create([FromBody] TCreate dto, CancellationToken ct = default)
        //{

        //}

        //[HttpPut("{id}")]
        //public virtual async Task<IActionResult> Update([FromRoute] TKey id, [FromBody] TUpdate dto, CancellationToken ct = default)
        //{

        //}

        //[HttpDelete("{id}")]
        //public virtual async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct = default)
        //{

        //}
    }
}
