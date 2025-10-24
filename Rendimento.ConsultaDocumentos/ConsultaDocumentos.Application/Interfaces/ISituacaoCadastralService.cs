using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Results;
using ConsultaDocumentos.Domain.Entities;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface ISituacaoCadastralService : IBaseService<SituacaoCadastralDTO, SituacaoCadastral>
    {
        Task<Result<IEnumerable<SituacaoCadastralDTO>>> GetByTipoPessoaAsync(char tipoPessoa);
    }
}
