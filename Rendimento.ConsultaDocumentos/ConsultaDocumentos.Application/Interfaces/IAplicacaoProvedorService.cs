using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Results;
using ConsultaDocumentos.Domain.Entities;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface IAplicacaoProvedorService : IBaseService<AplicacaoProvedorDTO, AplicacaoProvedor>
    {
        Task<Result<IList<AplicacaoProvedorDTO>>> GetByAplicacaoIdAsync(Guid aplicacaoId);
    }
}
