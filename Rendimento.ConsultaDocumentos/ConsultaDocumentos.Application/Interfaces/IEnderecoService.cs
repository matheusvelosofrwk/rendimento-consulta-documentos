using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Domain.Entities;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface IEnderecoService : IBaseService<EnderecoDTO, Endereco>
    {
    }
}
