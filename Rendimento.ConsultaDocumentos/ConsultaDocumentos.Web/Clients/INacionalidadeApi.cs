using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface INacionalidadeApi
    {
        [Get("/Nacionalidade")]
        Task<Result<IList<NacionalidadeViewModel>>> GetAllAsync();

        [Get("/Nacionalidade/{id}")]
        Task<Result<NacionalidadeViewModel>> GetByIdAsync(Guid id);

        [Post("/Nacionalidade")]
        Task<Result<NacionalidadeViewModel>> CreateAsync([Body] NacionalidadeViewModel model);

        [Put("/Nacionalidade/{id}")]
        Task<Result<NacionalidadeViewModel>> UpdateAsync(Guid id, [Body] NacionalidadeViewModel model);

        [Delete("/Nacionalidade/{id}")]
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}
