using ConsultaDocumentos.Domain.Base;
using ConsultaDocumentos.Domain.Entities;

namespace ConsultaDocumentos.Domain.Intefaces
{
    public interface ISituacaoCadastralRepository : IBaseRepository<SituacaoCadastral>
    {
        Task<IEnumerable<SituacaoCadastral>> GetByTipoPessoaAsync(char tipoPessoa);
    }
}
