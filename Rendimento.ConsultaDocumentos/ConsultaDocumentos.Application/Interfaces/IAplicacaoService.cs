using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Domain.Entities;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface IAplicacaoService : IBaseService<AplicacaoDTO, Aplicacao>
    {
    }
}
