using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface ISituacaoCadastralApi
    {
        [Get("/SituacaoCadastral")]
        Task<Result<IList<SituacaoCadastralViewModel>>> GetAllAsync();

        [Get("/SituacaoCadastral/tipo/{tipoPessoa}")]
        Task<Result<IList<SituacaoCadastralViewModel>>> GetByTipoPessoaAsync(char tipoPessoa);

        [Get("/SituacaoCadastral/{id}")]
        Task<Result<SituacaoCadastralViewModel>> GetByIdAsync(Guid id);

        [Post("/SituacaoCadastral")]
        Task<Result<SituacaoCadastralViewModel>> CreateAsync([Body] SituacaoCadastralViewModel model);

        [Put("/SituacaoCadastral/{id}")]
        Task<Result<SituacaoCadastralViewModel>> UpdateAsync(Guid id, [Body] SituacaoCadastralViewModel model);

        [Delete("/SituacaoCadastral/{id}")]
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}
